using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeVideoStore : IVideoStore
{
    public List<TrackedVideo> Videos { get; } = [];

    public Dictionary<string, DateOnly> PublishedOn { get; } = [];

    public bool FailRecordingFailures { get; set; }

    /// <summary>Runs right after a video is added, on the discovery side of a run.</summary>
    public Action<NewVideo>? OnAdded { get; set; }

    public Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            var known = Videos.Where(video => video.ChannelId == channelId).Select(video => video.YouTubeId).ToHashSet();
            return Task.FromResult<IReadOnlySet<string>>(known);
        }
    }

    public Task AddAsync(NewVideo video, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            Videos.Add(new TrackedVideo(Videos.Count + 1, video.ChannelId, video.YouTubeId, video.Title, video.State, 0, video.Reason, video.FirstSeenAt, video.PublishedOn));
            if (video.PublishedOn is { } publishedOn)
                PublishedOn[video.YouTubeId] = publishedOn;
        }

        OnAdded?.Invoke(video);
        return Task.CompletedTask;
    }

    public Task SetPublishedOnIfMissingAsync(string youTubeId, DateOnly publishedOn, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            PublishedOn.TryAdd(youTubeId, publishedOn);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            IReadOnlyList<TrackedVideo> listed =
            [
                .. Videos.Where(video => video.State == state)
                    .OrderBy(video => deferred.ContainsKey(video.Id))
                    .ThenBy(video => deferred.GetValueOrDefault(video.Id)),
            ];
            return Task.FromResult(listed);
        }
    }

    public Task DeferAsync(long videoId, DateTimeOffset deferredAt, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            deferred[videoId] = deferredAt;
        }

        return Task.CompletedTask;
    }

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken)
    {
        Replace(videoId, video => video with { State = state, LastReason = reason });
        return Task.CompletedTask;
    }

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken)
    {
        if (FailRecordingFailures)
            throw new InvalidOperationException("database is locked");

        Replace(videoId, video => video with { Attempts = video.Attempts + 1, LastReason = reason });
        return Task.CompletedTask;
    }

    public TrackedVideo Find(string youTubeId) => Videos.Single(video => video.YouTubeId == youTubeId);

    //

    private readonly Lock gate = new();
    private readonly Dictionary<long, DateTimeOffset> deferred = [];

    private void Replace(long videoId, Func<TrackedVideo, TrackedVideo> change)
    {
        lock (gate)
        {
            var index = Videos.FindIndex(video => video.Id == videoId);
            Videos[index] = change(Videos[index]);
        }
    }
}