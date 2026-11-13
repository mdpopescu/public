using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

public sealed class VideoDiscovery : IVideoDiscovery
{
    public const int RECHECK_DAYS = 7;
    public const int LOOK_BACK_MONTHS = 1;

    /// <summary>From this age YouTube's listing may say "1 month ago" for a video up to two months old, so the date must be confirmed.</summary>
    public const int LISTING_DATE_TRUSTED_DAYS = 28;

    public VideoDiscovery(IChannelStore channels, IVideoStore videos, IVideoCatalog catalog, TimeProvider time)
    {
        this.channels = channels;
        this.videos = videos;
        this.catalog = catalog;
        this.time = time;
    }

    public async Task<IReadOnlyList<string>> DiscoverAsync(IDiscoveryListener listener, CancellationToken cancellationToken)
    {
        var problems = new List<string>();
        await RecheckWaitingVideosAsync(problems, listener, cancellationToken).ConfigureAwait(false);

        var followed = await channels.ListAsync(cancellationToken).ConfigureAwait(false);
        var queue = new Queue<Channel>(followed);
        var checkedCount = 0;
        while (queue.TryDequeue(out var channel))
        {
            listener.ChannelStarted(checkedCount + 1, followed.Count, channel.Name);
            var refusal = await TryDiscoverChannelAsync(channel, listener, problems, cancellationToken).ConfigureAwait(false);
            if (refusal is null)
            {
                checkedCount++;
                listener.ChannelChecked();
                continue;
            }

            queue.Enqueue(channel);
            await listener.WaitOutChannelRateLimitAsync(refusal, cancellationToken).ConfigureAwait(false);
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

    private static bool NeedsConfirming(VideoDetails listed, DateOnly today) =>
        listed.Availability == VideoAvailability.Eligible
        && (listed.PublishedOn is null || listed.PublishedOn <= today.AddDays(-LISTING_DATE_TRUSTED_DAYS));

    /// <summary>
    ///     Checks one channel, recording any failure as a problem. Returns what YouTube answered when its rate limit refused
    ///     the check, so the channel goes to the end of the queue, or null when the check went ahead. The videos it recorded
    ///     before the refusal stay recorded.
    /// </summary>
    private async Task<string?> TryDiscoverChannelAsync(Channel channel, IDiscoveryListener listener, List<string> problems, CancellationToken cancellationToken)
    {
        try
        {
            await DiscoverChannelAsync(channel, listener, cancellationToken).ConfigureAwait(false);
        }
        catch (RateLimitedException exception)
        {
            return exception.Message;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            problems.Add($"Could not check channel {channel.Name}: {exception.Message}");
        }

        return null;
    }

    private async Task DiscoverChannelAsync(Channel channel, IDiscoveryListener listener, CancellationToken cancellationToken)
    {
        var known = (await videos.GetKnownIdsAsync(channel.Id, cancellationToken).ConfigureAwait(false)).ToHashSet();
        var walk = new ChannelWalk(channel, known, DateOnly.FromDateTime(time.GetLocalNow().DateTime), listener);

        foreach (var feed in await catalog.ListVideoFeedsAsync(channel, cancellationToken).ConfigureAwait(false))
        {
            await WalkFeedAsync(walk, feed, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Walks down to the look-back cutoff rather than stopping at the first known video:
    /// a run stopped partway through a channel leaves older videos behind the known ones, and the next run has to find them.
    /// </summary>
    private async Task WalkFeedAsync(ChannelWalk walk, IReadOnlyList<FeedVideo> feed, CancellationToken cancellationToken)
    {
        foreach (var listed in feed)
        {
            if (listed.Details.PublishedOn < walk.Cutoff)
            {
                return;
            }

            var carryOn = walk.Known.Contains(listed.YouTubeId)
                ? await FillInPublishedOnAsync(walk, listed, cancellationToken).ConfigureAwait(false)
                : await TryRecordAsync(walk, listed, cancellationToken).ConfigureAwait(false);
            if (!carryOn)
            {
                return;
            }
        }
    }

    private async Task<bool> FillInPublishedOnAsync(ChannelWalk walk, FeedVideo listed, CancellationToken cancellationToken)
    {
        if (listed.Details.PublishedOn is { } publishedOn && !NeedsConfirming(listed.Details, walk.Today))
        {
            await videos.SetPublishedOnIfMissingAsync(listed.YouTubeId, publishedOn, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    private async Task<bool> TryRecordAsync(ChannelWalk walk, FeedVideo listed, CancellationToken cancellationToken)
    {
        var details = NeedsConfirming(listed.Details, walk.Today)
            ? await catalog.GetDetailsAsync(listed.YouTubeId, cancellationToken).ConfigureAwait(false)
            : listed.Details;
        if (details.PublishedOn < walk.Cutoff)
        {
            return false;
        }

        var state = InitialStateFor(details.Availability);
        var video = new NewVideo(walk.Channel.Id, listed.YouTubeId, details.Title, state, details.Reason, time.GetUtcNow(), details.PublishedOn);
        await videos.AddAsync(video, cancellationToken).ConfigureAwait(false);
        walk.Known.Add(listed.YouTubeId);
        if (state == VideoState.Pending)
        {
            walk.Listener.VideoQueued();
        }

        return true;
    }

    private async Task RecheckWaitingVideosAsync(List<string> problems, IDiscoveryListener listener, CancellationToken cancellationToken)
    {
        foreach (var video in await videos.ListByStateAsync(VideoState.Recheck, cancellationToken).ConfigureAwait(false))
        {
            try
            {
                await RecheckAsync(video, listener, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                problems.Add($"Could not recheck video {video.Title}: {exception.Message}");
            }
        }
    }

    private async Task RecheckAsync(TrackedVideo video, IDiscoveryListener listener, CancellationToken cancellationToken)
    {
        var details = await catalog.GetDetailsAsync(video.YouTubeId, cancellationToken).ConfigureAwait(false);
        var expired = time.GetUtcNow() - video.FirstSeenAt > TimeSpan.FromDays(RECHECK_DAYS);
        var state = InitialStateFor(details.Availability);
        if (state == VideoState.Recheck && expired)
        {
            state = VideoState.GaveUp;
        }

        await videos.SetStateAsync(video.Id, state, details.Reason, cancellationToken).ConfigureAwait(false);
        if (state == VideoState.Pending)
        {
            listener.VideoQueued();
        }
    }

    private sealed record ChannelWalk(Channel Channel, HashSet<string> Known, DateOnly Today, IDiscoveryListener Listener)
    {
        public DateOnly Cutoff { get; } = Today.AddMonths(-LOOK_BACK_MONTHS);
    }
}