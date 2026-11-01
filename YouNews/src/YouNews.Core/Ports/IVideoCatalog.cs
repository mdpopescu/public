using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Finds channels and videos on YouTube.</summary>
public interface IVideoCatalog
{
    /// <summary>Resolves a channel URL or @handle. Returns null when no such channel exists.</summary>
    Task<ChannelDetails?> ResolveChannelAsync(string urlOrHandle, CancellationToken cancellationToken);

    /// <summary>Lists a channel's video IDs, one newest-first list per tab (videos, streams). A missing tab gives an empty list.</summary>
    Task<IReadOnlyList<IReadOnlyList<string>>> ListVideoFeedsAsync(Channel channel, CancellationToken cancellationToken);

    /// <summary>Gets the current details of a video. A video YouTube refuses to show is reported through its availability, not an exception.</summary>
    Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken);
}