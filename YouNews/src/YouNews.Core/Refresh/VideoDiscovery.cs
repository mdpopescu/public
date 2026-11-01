using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

public sealed class VideoDiscovery : IVideoDiscovery
{
    public const int RECHECK_DAYS = 7;
    public const int LOOK_BACK_MONTHS = 1;

    public VideoDiscovery(IChannelStore channels, IVideoStore videos, IVideoCatalog catalog, TimeProvider time)
    {
        this.channels = channels;
        this.videos = videos;
        this.catalog = catalog;
        this.time = time;
    }

    public async Task<IReadOnlyList<string>> DiscoverAsync(CancellationToken cancellationToken)
    {
        await RecheckWaitingVideosAsync(cancellationToken).ConfigureAwait(false);

        var problems = new List<string>();
        foreach (var channel in await channels.ListAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                await DiscoverChannelAsync(channel, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                problems.Add($"Could not check channel {channel.Name}: {exception.Message}");
            }
        }

        return problems;
    }

    //

    private readonly IChannelStore channels;
    private readonly IVideoStore videos;
    private readonly IVideoCatalog catalog;
    private readonly TimeProvider time;

    private static VideoState InitialStateFor(VideoAvailability availability)
    {
        return availability switch
        {
            VideoAvailability.Eligible => VideoState.Pending,
            VideoAvailability.RecheckLater => VideoState.Recheck,
            _ => VideoState.Skipped,
        };
    }

    private async Task DiscoverChannelAsync(Channel channel, CancellationToken cancellationToken)
    {
        var known = (await videos.GetKnownIdsAsync(channel.Id, cancellationToken).ConfigureAwait(false)).ToHashSet();
        var cutoff = DateOnly.FromDateTime(time.GetLocalNow().DateTime).AddMonths(-LOOK_BACK_MONTHS);

        foreach (var feed in await catalog.ListVideoFeedsAsync(channel, cancellationToken).ConfigureAwait(false))
        {
            await WalkFeedAsync(channel, feed, known, cutoff, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task WalkFeedAsync(Channel channel, IReadOnlyList<string> feed, HashSet<string> known, DateOnly cutoff, CancellationToken cancellationToken)
    {
        foreach (var youTubeId in feed)
        {
            if (known.Contains(youTubeId))
            {
                return;
            }

            var details = await catalog.GetDetailsAsync(youTubeId, cancellationToken).ConfigureAwait(false);
            if (details.PublishedOn < cutoff)
            {
                return;
            }

            var video = new NewVideo(channel.Id, youTubeId, details.Title, InitialStateFor(details.Availability), details.Reason, time.GetUtcNow());
            await videos.AddAsync(video, cancellationToken).ConfigureAwait(false);
            known.Add(youTubeId);
        }
    }

    private async Task RecheckWaitingVideosAsync(CancellationToken cancellationToken)
    {
        foreach (var video in await videos.ListByStateAsync(VideoState.Recheck, cancellationToken).ConfigureAwait(false))
        {
            var details = await catalog.GetDetailsAsync(video.YouTubeId, cancellationToken).ConfigureAwait(false);
            var expired = time.GetUtcNow() - video.FirstSeenAt > TimeSpan.FromDays(RECHECK_DAYS);
            var state = InitialStateFor(details.Availability);
            if (state == VideoState.Recheck && expired)
            {
                state = VideoState.GaveUp;
            }

            await videos.SetStateAsync(video.Id, state, details.Reason, cancellationToken).ConfigureAwait(false);
        }
    }
}