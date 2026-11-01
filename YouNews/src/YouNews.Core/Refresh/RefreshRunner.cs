using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Setup;

namespace YouNews.Core.Refresh;

public sealed class RefreshRunner : IRefreshRun
{
    public RefreshRunner(RefreshDependencies dependencies, IVideoDiscovery discovery, IVideoProcessor processor, RefreshLimits limits)
    {
        this.dependencies = dependencies;
        this.discovery = discovery;
        this.processor = processor;
        this.limits = limits;
    }

    public async Task ExecuteAsync(Action<RefreshProgress> reportProgress, CancellationToken cancellationToken)
    {
        var startedAt = dependencies.Time.GetUtcNow();

        reportProgress(new RefreshProgress("Checking the AI server's setup", 0, 0));
        var setup = await dependencies.SetupMonitor.GetCurrentAsync(cancellationToken).ConfigureAwait(false);
        var interests = await dependencies.Interests.GetAsync(cancellationToken).ConfigureAwait(false);

        reportProgress(new RefreshProgress("Looking for new videos", 0, 0));
        var problems = await discovery.DiscoverAsync(cancellationToken).ConfigureAwait(false);

        var channels = await dependencies.Channels.ListAsync(cancellationToken).ConfigureAwait(false);
        var pending = await dependencies.Videos.ListByStateAsync(VideoState.Pending, cancellationToken).ConfigureAwait(false);
        var toProcess = limits.MaximumVideosPerRun is { } maximum ? pending.Take(maximum).ToList() : pending;

        var context = new ProcessingContext(channels.ToDictionary(channel => channel.Id), setup, interests);
        var (completed, failed) = await ProcessAllAsync(toProcess, context, reportProgress, cancellationToken).ConfigureAwait(false);

        var problem = problems.Count == 0 ? null : string.Join(" ", problems);
        var finished = new RefreshRun(startedAt, dependencies.Time.GetUtcNow(), completed, failed, problem);
        await dependencies.Log.RecordAsync(finished, cancellationToken).ConfigureAwait(false);
    }

    //

    private readonly RefreshDependencies dependencies;
    private readonly IVideoDiscovery discovery;
    private readonly IVideoProcessor processor;
    private readonly RefreshLimits limits;

    private async Task<(int Completed, int Failed)> ProcessAllAsync(
        IReadOnlyList<TrackedVideo> videos,
        ProcessingContext context,
        Action<RefreshProgress> reportProgress,
        CancellationToken cancellationToken)
    {
        var completed = 0;
        var failed = 0;
        foreach (var video in videos)
        {
            reportProgress(new RefreshProgress(video.Title, completed + failed, videos.Count));
            try
            {
                await processor.ProcessAsync(video, context, cancellationToken).ConfigureAwait(false);
                completed++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                await dependencies.Videos.RecordFailureAsync(video.Id, exception.Message, cancellationToken).ConfigureAwait(false);
                failed++;
            }
        }

        return (completed, failed);
    }
}

/// <summary>The stores and services a refresh run reads and reports through.</summary>
public sealed record RefreshDependencies(
    IChannelStore Channels,
    IVideoStore Videos,
    IInterestStore Interests,
    IRefreshLog Log,
    ISetupMonitor SetupMonitor,
    TimeProvider Time);