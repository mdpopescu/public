using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeCatalog : IVideoCatalog
{
    /// <summary>Feeds that give only IDs, so discovery has to ask for the details of each video.</summary>
    public Dictionary<long, IReadOnlyList<IReadOnlyList<string>>> Feeds { get; } = [];

    /// <summary>Feeds as a real listing gives them, with what YouTube shows for each video.</summary>
    public Dictionary<long, IReadOnlyList<IReadOnlyList<FeedVideo>>> ListedFeeds { get; } = [];

    public Dictionary<string, VideoDetails> Details { get; } = [];

    public HashSet<long> FailingChannels { get; } = [];

    public List<string> DetailRequests { get; } = [];

    /// <summary>When false, asking about a video without details fails, as a failing lookup does.</summary>
    public bool UnknownVideosHaveNoDate { get; set; }

    public Task<ChannelDetails?> ResolveChannelAsync(string urlOrHandle, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<IReadOnlyList<IReadOnlyList<FeedVideo>>> ListVideoFeedsAsync(Channel channel, CancellationToken cancellationToken)
    {
        if (FailingChannels.Contains(channel.Id))
            throw new InvalidOperationException("listing failed");

        if (ListedFeeds.TryGetValue(channel.Id, out var listed))
            return Task.FromResult(listed);

        IReadOnlyList<IReadOnlyList<FeedVideo>> idsOnly =
        [
            .. Feeds[channel.Id]
                .Select(feed => (IReadOnlyList<FeedVideo>)[.. feed.Select(id => new FeedVideo(id, new VideoDetails(id, VideoAvailability.Eligible, null, null)))]),
        ];
        return Task.FromResult(idsOnly);
    }

    public Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken)
    {
        DetailRequests.Add(youTubeId);
        return Task.FromResult(
            UnknownVideosHaveNoDate && !Details.ContainsKey(youTubeId)
                ? new VideoDetails(youTubeId, VideoAvailability.Eligible, null, null)
                : Details[youTubeId]);
    }
}