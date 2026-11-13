using System.Diagnostics.CodeAnalysis;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

public sealed class RefreshRunner(RefreshDependencies dependencies, IVideoDiscovery discovery, IVideoProcessor processor, RefreshLimits limits)
    : IRefreshRun
{
    /// <summary>A video that has failed this many times is given up on, so that a broken one is not retried forever.</summary>
    public const int MAXIMUM_ATTEMPTS = 15;

    /// <summary>How long YouTube is left alone after it refuses a request because of its rate limit.</summary>
    public static readonly TimeSpan RATE_LIMIT_PAUSE = TimeSpan.FromMinutes(10);

    /// <summary>
    /// The run stops when YouTube has refused this many requests in a row, counting both the search for new videos and the processing.
    /// The refusal that reaches this number is not waited out.
    /// </summary>
    public const int MAXIMUM_RATE_LIMIT_PAUSES_IN_A_ROW = 5;

    public async Task ExecuteAsync(Action<RefreshProgress> reportProgress, CancellationToken cancellationToken)
    {
        var startedAt = dependencies.Time.GetUtcNow();

        reportProgress(new RefreshProgress("Checking the AI server's setup", 0, 0));
        var setup = await dependencies.SetupMonitor.GetCurrentAsync(cancellationToken).ConfigureAwait(false);
        var interests = await dependencies.Interests.GetAsync(cancellationToken).ConfigureAwait(false);
        var channels = await dependencies.Channels.ListAsync(cancellationToken).ConfigureAwait(false);
        var context = new ProcessingContext(channels.ToDictionary(channel => channel.Id), setup, interests);

        using var activity = new RefreshActivity(reportProgress, dependencies.Time);
        using var discoveryStop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, activity.RateLimitStop);
        // ReSharper disable once AccessToDisposedClosure
        var discovering = Task.Run(() => DiscoverAsync(activity, discoveryStop.Token, cancellationToken), CancellationToken.None);

        var outcome = await ProcessAlongsideDiscoveryAsync(activity, context, new RunningDiscovery(discovering, discoveryStop), cancellationToken).ConfigureAwait(false);
        var problems = await discovering.ConfigureAwait(false);

        await FinishAsync(activity, startedAt, outcome, problems, cancellationToken).ConfigureAwait(false);
    }

    //

    private static string StoppedProblem(string? lastRefusal) =>
        $"The run was stopped because YouTube refused {MAXIMUM_RATE_LIMIT_PAUSES_IN_A_ROW} requests in a row. Videos still waiting will be tried again on the next refresh."
        + (lastRefusal is null ? "" : $" The last refusal was: {lastRefusal}");

    private static string RateLimitProblem(int pauses) =>
        pauses == 1
            ? $"YouTube's rate limit paused the run once for {RATE_LIMIT_PAUSE.TotalMinutes:0} minutes."
            : $"YouTube's rate limit paused the run {pauses} times for {RATE_LIMIT_PAUSE.TotalMinutes:0} minutes each.";

    private async Task FinishAsync(RefreshActivity activity, DateTimeOffset startedAt, QueueOutcome outcome, IReadOnlyList<string> discoveryProblems, CancellationToken cancellationToken)
    {
        var problems = discoveryProblems.ToList();
        if (activity.RateLimitPauses > 0)
            problems.Add(RateLimitProblem(activity.RateLimitPauses));
        var stopped = activity.StoppedByRateLimit ? StoppedProblem(activity.LastRefusal) : null;
        if (stopped is not null)
            problems.Add(stopped);

        var problem = problems.Count == 0 ? null : string.Join(" ", problems);
        var finishedAt = dependencies.Time.GetUtcNow();
        await dependencies.Log.RecordAsync(new RefreshRun(startedAt, finishedAt, outcome.Completed, outcome.Failed, problem), cancellationToken).ConfigureAwait(false);
        if (stopped is not null)
            await dependencies.Alerts.RaiseAsync(stopped, finishedAt, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<string>> DiscoverAsync(RefreshActivity activity, CancellationToken stopToken, CancellationToken cancellationToken)
    {
        try
        {
            return await discovery.DiscoverAsync(activity, stopToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (activity.StoppedByRateLimit && !cancellationToken.IsCancellationRequested)
        {
            return [];
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !stopToken.IsCancellationRequested)
        {
            return [$"Could not look for new videos: {exception.Message}"];
        }
        finally
        {
            activity.DiscoveryFinished();
        }
    }

    private async Task<QueueOutcome> ProcessAlongsideDiscoveryAsync(
        RefreshActivity activity,
        ProcessingContext context,
        RunningDiscovery running,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ProcessQueueAsync(activity, context, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await running.Stop.CancelAsync().ConfigureAwait(false);
            await Task.WhenAny(running.Finished).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>
    ///     The consumer side. The store is the queue, and it is asked again before every video,
    ///     so a video discovered a moment ago still goes ahead of older ones already waiting.
    ///     A video refused by YouTube's rate limit goes to the end of the queue, and the run leaves YouTube alone for a while
    ///     before taking the next one: asking again at once would only be refused and keep the limit in place.
    ///     The queue ends when YouTube has refused too many requests in a row, for the search and the processing together.
    /// </summary>
    private async Task<QueueOutcome> ProcessQueueAsync(RefreshActivity activity, ProcessingContext context, CancellationToken cancellationToken)
    {
        var tried = new HashSet<long>();
        var completed = 0;
        while (!activity.StoppedByRateLimit && (limits.MaximumVideosPerRun is not { } maximum || tried.Count < maximum))
        {
            var next = await StartNextVideoAsync(activity, tried, cancellationToken).ConfigureAwait(false);
            if (next is null || activity.StoppedByRateLimit)
                break;

            var (outcome, refusal) = await TryProcessAsync(next, context, cancellationToken).ConfigureAwait(false);
            if (outcome == VideoOutcome.RateLimited)
            {
                await dependencies.Videos.DeferAsync(next.Id, dependencies.Time.GetUtcNow(), cancellationToken).ConfigureAwait(false);
                await activity.WaitOutVideoRateLimitAsync(refusal, cancellationToken).ConfigureAwait(false);
                continue;
            }

            activity.VideoProcessed();
            tried.Add(next.Id);
            completed += outcome == VideoOutcome.Completed ? 1 : 0;
        }

        return new QueueOutcome(completed, tried.Count - completed);
    }

    /// <summary>
    ///     Gets the first waiting video, sleeping while the queue is empty. Returns null when discovery has ended and
    ///     none is left.
    /// </summary>
    private async Task<TrackedVideo?> StartNextVideoAsync(RefreshActivity activity, HashSet<long> tried, CancellationToken cancellationToken)
    {
        do
        {
            activity.ForgetEarlierWakeUps();
            var pending = await dependencies.Videos.ListByStateAsync(VideoState.Pending, cancellationToken).ConfigureAwait(false);
            var waiting = pending.Where(video => !tried.Contains(video.Id)).ToList();
            if (waiting.Count <= 0)
                continue;

            activity.VideoStarted(waiting[0].Title, tried.Count, TotalFor(tried.Count + waiting.Count));
            return waiting[0];
        }
        while (await activity.WaitForMoreVideosAsync(cancellationToken).ConfigureAwait(false));

        return null;
    }

    private int TotalFor(int videos) => limits.MaximumVideosPerRun is { } maximum ? Math.Min(maximum, videos) : videos;

    /// <summary>Processes one video. The text that comes back is what YouTube answered when its rate limit refused the video.</summary>
    private async Task<(VideoOutcome Outcome, string Refusal)> TryProcessAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken)
    {
        try
        {
            await processor.ProcessAsync(video, context, cancellationToken).ConfigureAwait(false);
            return (VideoOutcome.Completed, "");
        }
        catch (RateLimitedException exception)
        {
            return (VideoOutcome.RateLimited, exception.Message);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            await TryRecordFailureAsync(video, exception.Message, cancellationToken).ConfigureAwait(false);
            return (VideoOutcome.Failed, "");
        }
    }

    private async Task TryRecordFailureAsync(TrackedVideo video, string reason, CancellationToken cancellationToken)
    {
        try
        {
            await dependencies.Videos.RecordFailureAsync(video.Id, reason, cancellationToken).ConfigureAwait(false);
            if (video.Attempts + 1 >= MAXIMUM_ATTEMPTS)
                await dependencies.Videos.SetStateAsync(video.Id, VideoState.GaveUp, reason, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The video stays pending either way, so only the displayed reason is lost. The other videos still run.
        }
    }

    private sealed record RunningDiscovery(Task Finished, CancellationTokenSource Stop);

    private sealed record QueueOutcome(int Completed, int Failed);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private enum VideoOutcome
    {
        Completed,
        Failed,
        RateLimited,
    }
}