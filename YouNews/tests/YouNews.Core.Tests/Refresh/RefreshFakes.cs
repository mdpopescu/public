using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
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
            {
                PublishedOn[video.YouTubeId] = publishedOn;
            }
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
            return Task.FromResult<IReadOnlyList<TrackedVideo>>(Videos.Where(video => video.State == state).ToList());
        }
    }

    public Task SetStateAsync(long videoId, VideoState state, string? reason, CancellationToken cancellationToken)
    {
        Replace(videoId, video => video with { State = state, LastReason = reason });
        return Task.CompletedTask;
    }

    public Task RecordFailureAsync(long videoId, string reason, CancellationToken cancellationToken)
    {
        if (FailRecordingFailures)
        {
            throw new InvalidOperationException("database is locked");
        }

        Replace(videoId, video => video with { Attempts = video.Attempts + 1, LastReason = reason });
        return Task.CompletedTask;
    }

    public TrackedVideo Find(string youTubeId) => Videos.Single(video => video.YouTubeId == youTubeId);

    //

    private readonly Lock gate = new();

    private void Replace(long videoId, Func<TrackedVideo, TrackedVideo> change)
    {
        lock (gate)
        {
            var index = Videos.FindIndex(video => video.Id == videoId);
            Videos[index] = change(Videos[index]);
        }
    }
}

internal sealed class FakeDiscoveryListener : IDiscoveryListener
{
    public List<string> Channels { get; } = [];

    public int VideosQueued { get; private set; }

    public void ChannelStarted(int position, int total, string channelName) => Channels.Add($"{position}/{total} {channelName}");

    public void VideoQueued() => VideosQueued++;
}

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
        {
            throw new InvalidOperationException("listing failed");
        }

        if (ListedFeeds.TryGetValue(channel.Id, out var listed))
        {
            return Task.FromResult(listed);
        }

        IReadOnlyList<IReadOnlyList<FeedVideo>> idsOnly = Feeds[channel.Id]
            .Select(feed => (IReadOnlyList<FeedVideo>)feed.Select(id => new FeedVideo(id, new VideoDetails(id, VideoAvailability.Eligible, null, null))).ToList())
            .ToList();
        return Task.FromResult(idsOnly);
    }

    public Task<VideoDetails> GetDetailsAsync(string youTubeId, CancellationToken cancellationToken)
    {
        DetailRequests.Add(youTubeId);
        return Task.FromResult(UnknownVideosHaveNoDate && !Details.ContainsKey(youTubeId)
            ? new VideoDetails(youTubeId, VideoAvailability.Eligible, null, null)
            : Details[youTubeId]);
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
    public HashSet<string> FailingTitles { get; } = [];

    public VideoAnalysis Analysis { get; set; } = new("Headline", ["research"], [TimeSpan.FromSeconds(30)], [], []);

    public OpinionRequest? LastOpinionRequest { get; private set; }

    public Transcript? LastTranscript { get; private set; }

    public IReadOnlyList<EntryDigest>? LastCandidates { get; private set; }

    public string Summary { get; set; } = "Summary";

    public string Opinion { get; set; } = "Opinion";

    /// <summary>Everything that happened, in order, as "analyze title" or "opinion title".</summary>
    public List<string> Events { get; } = [];

    /// <summary>Runs while an opinion is being written, so a test can hold the writer.</summary>
    public Func<Task>? DuringOpinion { get; set; }

    public Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
    {
        Events.Add("analyze " + title);
        LastTranscript = transcript;
        LastCandidates = candidates;
        return FailingTitles.Contains(title) ? throw new InvalidOperationException("model is down") : Task.FromResult(Analysis);
    }

    public Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken) => Task.FromResult(Summary);

    public async Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken)
    {
        Events.Add("opinion " + request.Title);
        LastOpinionRequest = request;
        if (DuringOpinion is not null)
        {
            await DuringOpinion().ConfigureAwait(false);
        }

        Events.Add("opinion done " + request.Title);
        return Opinion;
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
    public int Captures { get; private set; }

    public IReadOnlyList<EntryImage> Images { get; set; } = [new EntryImage("id/thumbnail.jpg", ImageKind.Thumbnail)];

    public Task<IReadOnlyList<EntryImage>> CaptureAsync(string youTubeId, IReadOnlyList<TimeSpan> frameTimes, CancellationToken cancellationToken)
    {
        Captures++;
        return Task.FromResult(Images);
    }
}

internal sealed class FakeEntryStore : IEntryStore
{
    public List<NewEntry> Added { get; } = [];

    public List<EntryDigest> Digests { get; } = [];

    public List<EntryRevision> Revised { get; } = [];

    public Action<long>? OnAdded { get; set; }

    public Task<long> AddAsync(NewEntry entry, CancellationToken cancellationToken)
    {
        Added.Add(entry);
        OnAdded?.Invoke(entry.VideoId);
        return Task.FromResult((long)Added.Count);
    }

    public Task ReviseAsync(EntryRevision revision, CancellationToken cancellationToken)
    {
        Revised.Add(revision);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EntryDigest>> ListRecentDigestsAsync(int limit, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EntryDigest>>(Digests);
}

internal sealed class FakeReprocessQueue : IReprocessQueue
{
    public Dictionary<long, string> Failures { get; } = [];

    public int GetNextCalls { get; private set; }

    /// <summary>The number of GetNextAsync calls that fail, as a locked database does.</summary>
    public int FailingCalls { get; set; }

    public Action? OnGetNextFailed { get; set; }

    public List<long> Queued
    {
        get
        {
            lock (gate)
            {
                return requests.ToList();
            }
        }
    }

    public Task RequestAsync(long entryId, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            if (!requests.Contains(entryId))
            {
                requests.Add(entryId);
            }

            Failures.Remove(entryId);
        }

        return Task.CompletedTask;
    }

    public Task<ReprocessJob?> GetNextAsync(CancellationToken cancellationToken)
    {
        lock (gate)
        {
            GetNextCalls++;
            if (GetNextCalls <= FailingCalls)
            {
                OnGetNextFailed?.Invoke();
                throw new InvalidOperationException("database is locked");
            }

            var next = requests.Where(id => !Failures.ContainsKey(id)).Select(id => (long?)id).FirstOrDefault();
            return Task.FromResult(next is { } id ? new ReprocessJob(id, $"video{id}", $"Title {id}", "Channel One") : null);
        }
    }

    public Task RecordFailureAsync(long entryId, string reason, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            Failures[entryId] = reason;
        }

        return Task.CompletedTask;
    }

    public void Complete(long entryId)
    {
        lock (gate)
        {
            requests.Remove(entryId);
        }
    }

    //

    private readonly Lock gate = new();
    private readonly List<long> requests = [];
}

internal sealed class FakeEntryReprocessor(FakeReprocessQueue queue) : IEntryReprocessor
{
    public List<long> Processed { get; } = [];

    public List<SetupSnapshot?> Setups { get; } = [];

    /// <summary>What the reprocess does; by default it succeeds at once.</summary>
    public Func<ReprocessJob, CancellationToken, Task> Handler { get; set; } = (_, _) => Task.CompletedTask;

    public async Task ReprocessAsync(ReprocessJob job, SetupSnapshot? setup, IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        Setups.Add(setup);
        await Handler(job, cancellationToken).ConfigureAwait(false);
        queue.Complete(job.EntryId);
        Processed.Add(job.EntryId);
    }
}

internal sealed class FailingSetupMonitor : ISetupMonitor
{
    public Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken) => throw new InvalidOperationException("setup unavailable");
}