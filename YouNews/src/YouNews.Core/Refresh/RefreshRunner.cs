using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

public sealed class RefreshRunner(RefreshDependencies dependencies, IVideoDiscovery discovery, IVideoProcessor processor, RefreshLimits limits)
    : IRefreshRun
{
    public async Task ExecuteAsync(Action<RefreshProgress> reportProgress, CancellationToken cancellationToken)
    {
        var startedAt = dependencies.Time.GetUtcNow();

        reportProgress(new RefreshProgress("Checking the AI server's setup", 0, 0));
        var setup = await dependencies.SetupMonitor.GetCurrentAsync(cancellationToken).ConfigureAwait(false);
        var interests = await dependencies.Interests.GetAsync(cancellationToken).ConfigureAwait(false);
        var channels = await dependencies.Channels.ListAsync(cancellationToken).ConfigureAwait(false);
        var context = new ProcessingContext(channels.ToDictionary(channel => channel.Id), setup, interests);

        var activity = new RefreshActivity(reportProgress);
        using var discoveryStop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        // ReSharper disable once AccessToDisposedClosure
        var discovering = Task.Run(() => DiscoverAsync(activity, discoveryStop.Token), CancellationToken.None);

        var (completed, failed) = await ProcessAlongsideDiscoveryAsync(activity, context, new RunningDiscovery(discovering, discoveryStop), cancellationToken).ConfigureAwait(false);
        var problems = await discovering.ConfigureAwait(false);

        var problem = problems.Count == 0 ? null : string.Join(" ", problems);
        var finished = new RefreshRun(startedAt, dependencies.Time.GetUtcNow(), completed, failed, problem);
        await dependencies.Log.RecordAsync(finished, cancellationToken).ConfigureAwait(false);
    }

    //

    private async Task<IReadOnlyList<string>> DiscoverAsync(RefreshActivity activity, CancellationToken cancellationToken)
    {
        try
        {
            return await discovery.DiscoverAsync(activity, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            return [$"Could not look for new videos: {exception.Message}"];
        }
        finally
        {
            activity.DiscoveryFinished();
        }
    }

    private async Task<(int Completed, int Failed)> ProcessAlongsideDiscoveryAsync(
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
    /// </summary>
    private async Task<(int Completed, int Failed)> ProcessQueueAsync(RefreshActivity activity, ProcessingContext context, CancellationToken cancellationToken)
    {
        var tried = new HashSet<long>();
        var completed = 0;
        while (limits.MaximumVideosPerRun is not { } maximum || tried.Count < maximum)
        {
            var next = await StartNextVideoAsync(activity, tried, cancellationToken).ConfigureAwait(false);
            if (next is null)
                break;

            tried.Add(next.Id);
            completed += await TryProcessAsync(next, context, cancellationToken).ConfigureAwait(false) ? 1 : 0;
        }

        return (completed, tried.Count - completed);
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

    private async Task<bool> TryProcessAsync(TrackedVideo video, ProcessingContext context, CancellationToken cancellationToken)
    {
        try
        {
            await processor.ProcessAsync(video, context, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            await TryRecordFailureAsync(video, exception.Message, cancellationToken).ConfigureAwait(false);
            return false;
        }
    }

    private async Task TryRecordFailureAsync(TrackedVideo video, string reason, CancellationToken cancellationToken)
    {
        try
        {
            await dependencies.Videos.RecordFailureAsync(video.Id, reason, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The video stays pending either way, so only the displayed reason is lost. The other videos still run.
        }
    }

    private sealed record RunningDiscovery(Task Finished, CancellationTokenSource Stop);
}