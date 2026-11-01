using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Keeps every video seen and its processing state.</summary>
public interface IVideoStore
{
    /// <summary>Gets the YouTube IDs of all videos already seen for a channel, in any state.</summary>
    Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken);

    /// <summary>Records a newly seen video. A video already known is left unchanged.</summary>
    Task AddAsync(NewVideo video, CancellationToken cancellationToken);

    /// <summary>Records when a known video was published, unless that is already recorded.</summary>
    Task SetPublishedOnIfMissingAsync(string youTubeId, DateOnly publishedOn, CancellationToken cancellationToken);

    /// <summary>
    /// Lists videos in a state, leaving out those of channels no longer followed. The most recently discovered day comes first,
    /// and within a day the most recently published video. Videos without a known publication date come last in their day.
    /// </summary>
    Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken);

    /// <summary>Changes the state of a video and records why, without counting an attempt.</summary>
    Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken);

    /// <summary>Records a failed processing attempt. The video stays pending.</summary>
    Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken);
}