using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Setup;

namespace YouNews.Core.Tests.Refresh;

internal sealed class FakeChannelStore : IChannelStore
{
    public List<Channel> Channels { get; } = [];

    public Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Channel>>(Channels.ToList());

    public Task<Channel?> AddAsync(ChannelDetails details, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task RemoveAsync(long channelId, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeVideoStore : IVideoStore
{
    public List<TrackedVideo> Videos { get; } = [];

    public Task<IReadOnlySet<string>> GetKnownIdsAsync(long channelId, CancellationToken cancellationToken)
    {
        var known = Videos.Where(video => video.ChannelId == channelId).Select(video => video.YouTubeId).ToHashSet();
        return Task.FromResult<IReadOnlySet<string>>(known);
    }

    public Task AddAsync(NewVideo video, CancellationToken cancellationToken)
    {
        Videos.Add(new TrackedVideo(Videos.Count + 1, video.ChannelId, video.YouTubeId, video.Title, video.State, 0, video.Reason, video.FirstSeenAt));
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TrackedVideo>> ListByStateAsync(VideoState state, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<TrackedVideo>>(Videos.Where(video => video.State == state).ToList());
    }

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken)
    {
        Replace(videoId, video => video with { State = state, LastReason = reason });
        return Task.CompletedTask;
    }

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken)
    {
        Replace(videoId, video => video with { Attempts = video.Attempts + 1, LastReason = reason });
        return Task.CompletedTask;
    }

    public TrackedVideo Find(string youTubeId) => Videos.Single(video => video.YouTubeId == youTubeId);

    //

    private void Replace(long videoId, Func<TrackedVideo, TrackedVideo> change)
    {
        var index = Videos.FindIndex(video => video.Id == videoId);
        Videos[index] = change(Videos[index]);
    }
}

internal sealed class FakeCatalog : IVideoCatalog
{
    public Dictionary<long, IReadOnlyList<IReadOnlyList<string>>> Feeds { get; } = [];

    public Dictionary<string, VideoDetails> Details { get; } = [];

    public HashSet<long> FailingChannels { get; } = [];

    public List<string> DetailRequests { get; } = [];

    public Task<ChannelDetails?> ResolveChannelAsync(string urlOrHandle, CancellationToken cancellationToken) => throw new NotSupportedException();

    public Task<IReadOnlyList<IReadOnlyList<string>>> ListVideoFeedsAsync(Channel channel, CancellationToken cancellationToken)
    {
        if (FailingChannels.Contains(channel.Id))
        {
            throw new InvalidOperationException("listing failed");
        }

        return Task.FromResult(Feeds[channel.Id]);
    }

    public Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken)
    {
        DetailRequests.Add(youTubeId);
        return Task.FromResult(Details[youTubeId]);
    }
}

internal sealed class FakeInterestStore : IInterestStore
{
    public Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<string>>(["local models"]);

    public Task SaveAsync(IReadOnlyList<string> interests, CancellationToken cancellationToken) => throw new NotSupportedException();
}

internal sealed class FakeRefreshLog : IRefreshLog
{
    public List<RefreshRun> Runs { get; } = [];

    public Task RecordAsync(RefreshRun run, CancellationToken cancellationToken)
    {
        Runs.Add(run);
        return Task.CompletedTask;
    }

    public Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken) => Task.FromResult(Runs.LastOrDefault());
}

internal sealed class FakeSetupMonitor : ISetupMonitor
{
    public SetupSnapshot? Snapshot { get; set; }

    public Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken) => Task.FromResult(Snapshot);
}

internal sealed class FakeCaptionSource : ICaptionSource
{
    public Transcript? Result { get; set; } = new([new TranscriptSegment(TimeSpan.Zero, "words from captions")]);

    public Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken) => Task.FromResult(Result);
}

internal sealed class FakeSpeechTranscriber : ISpeechTranscriber
{
    public Transcript? Result { get; set; }

    public Task<Transcript> TranscribeAsync(string youTubeId, CancellationToken cancellationToken)
    {
        return Result is null ? throw new InvalidOperationException("speech unavailable") : Task.FromResult(Result);
    }
}

internal sealed class FakeBriefingWriter : IBriefingWriter
{
    public VideoAnalysis Analysis { get; set; } = new("Headline", ["research"], [TimeSpan.FromSeconds(30)], [], []);

    public HashSet<string> FailingTitles { get; } = [];

    public OpinionRequest? LastOpinionRequest { get; private set; }

    public Transcript? LastTranscript { get; private set; }

    public Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
    {
        LastTranscript = transcript;
        return FailingTitles.Contains(title) ? throw new InvalidOperationException("model is down") : Task.FromResult(Analysis);
    }

    public Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken) => Task.FromResult("Summary");

    public Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken)
    {
        LastOpinionRequest = request;
        return Task.FromResult("Opinion");
    }
}

internal sealed class FakeWebSearch : IWebSearch
{
    public List<WebResult> Results { get; } = [];

    public List<string> Queries { get; } = [];

    public Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        Queries.Add(query);
        return Task.FromResult<IReadOnlyList<WebResult>>(Results);
    }
}

internal sealed class FakeImageSource : IImageSource
{
    public IReadOnlyList<EntryImage> Images { get; set; } = [new EntryImage("id/thumbnail.jpg", ImageKind.Thumbnail)];

    public Task<IReadOnlyList<EntryImage>> CaptureAsync(string youTubeId, IReadOnlyList<TimeSpan> frameTimes, CancellationToken cancellationToken) => Task.FromResult(Images);
}

internal sealed class FakeEntryStore : IEntryStore
{
    public List<NewEntry> Added { get; } = [];

    public List<EntryDigest> Digests { get; } = [];

    public Action<long>? OnAdded { get; set; }

    public Task<long> AddAsync(NewEntry entry, CancellationToken cancellationToken)
    {
        Added.Add(entry);
        OnAdded?.Invoke(entry.VideoId);
        return Task.FromResult((long)Added.Count);
    }

    public Task<IReadOnlyList<EntryDigest>> ListRecentDigestsAsync(int limit, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EntryDigest>>(Digests);
}