using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class RefreshRunTests
{
    [TestInitialize]
    public void Initialize()
    {
        time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 23, 30, 0, TimeSpan.Zero));
        time.SetLocalTimeZone(TimeZoneInfo.CreateCustomTimeZone("Plus2", TimeSpan.FromHours(2), "Plus2", "Plus2"));
        channels = new FakeChannelStore();
        channels.Channels.Add(new Channel(1, "UC1", "https://youtube.com/@one", "Channel One"));
        videos = new FakeVideoStore();
        catalog = new FakeCatalog();
        catalog.Feeds[1] = [[]];
        log = new FakeRefreshLog();
        setupMonitor = new FakeSetupMonitor();
        captions = new FakeCaptionSource();
        speech = new FakeSpeechTranscriber();
        writer = new FakeBriefingWriter();
        search = new FakeWebSearch();
        images = new FakeImageSource();
        entries = new FakeEntryStore();
        entries.OnAdded = videoId => videos.SetStateAsync(videoId, VideoState.Done, null, CancellationToken.None);
        sut = CreateSut(limit: null);
    }

    [TestMethod]
    public async Task ProducesAnEntryDatedByRetrievalNotPublication()
    {
        await AddPendingAsync("video");

        await RunAsync();

        Assert.HasCount(1, entries.Added);
        var entry = entries.Added[0];
        Assert.AreEqual(new DateOnly(2026, 11, 2), entry.Day, "23:30 UTC is already the next day in local time.");
        Assert.AreEqual("Headline", entry.Headline);
        Assert.AreEqual("Summary", entry.Summary);
        Assert.AreEqual("Opinion", entry.Opinion);
        Assert.HasCount(1, entry.Images);
        Assert.AreEqual(VideoState.Done, videos.Find("video").State);
    }

    [TestMethod]
    public async Task OpinionReceivesSetupInterestsRelatedEntriesAndSearchResults()
    {
        var setup = new SetupSnapshot(["gemma4-26b"], "RTX 5060 Ti", time.GetUtcNow(), null);
        setupMonitor.Snapshot = setup;
        var relatedDigest = new EntryDigest(7, new DateOnly(2026, 10, 1), "Earlier", ["research"], "Earlier summary");
        entries.Digests.Add(relatedDigest);
        entries.Digests.Add(new EntryDigest(8, new DateOnly(2026, 10, 2), "Unrelated", ["research"], "Other"));
        writer.Analysis = new VideoAnalysis("Headline", ["research"], [], [7], ["query one"]);
        search.Results.Add(new WebResult("A source", "https://example.com/a", "snippet"));
        await AddPendingAsync("video");

        await RunAsync();

        var request = writer.LastOpinionRequest!;
        Assert.AreSame(setup, request.Setup);
        CollectionAssert.AreEqual(new[] { "local models" }, request.Interests.ToArray());
        Assert.AreEqual(relatedDigest, request.RelatedEntries.Single());
        Assert.AreEqual("A source", request.SearchResults.Single().Title);
        CollectionAssert.AreEqual(new long[] { 7 }, entries.Added[0].RelatedEntryIds.ToArray());
        Assert.AreEqual("https://example.com/a", entries.Added[0].WebSources.Single().Url);
    }

    [TestMethod]
    public async Task FailingVideoStaysPendingAndTheRunContinues()
    {
        await AddPendingAsync("bad");
        await AddPendingAsync("good");
        writer.FailingTitles.Add("Title of bad");

        await RunAsync();

        var bad = videos.Find("bad");
        Assert.AreEqual(VideoState.Pending, bad.State);
        Assert.AreEqual(1, bad.Attempts);
        Assert.AreEqual("model is down", bad.LastReason);
        Assert.AreEqual(VideoState.Done, videos.Find("good").State);
        Assert.AreEqual(1, log.Runs[0].Completed);
        Assert.AreEqual(1, log.Runs[0].Failed);
    }

    [TestMethod]
    public async Task ModelDownForTheWholeRunLosesNothing()
    {
        await AddPendingAsync("one");
        await AddPendingAsync("two");
        writer.FailingTitles.UnionWith(["Title of one", "Title of two"]);

        await RunAsync();
        writer.FailingTitles.Clear();
        await RunAsync();

        Assert.HasCount(2, entries.Added);
        Assert.AreEqual(2, log.Runs[1].Completed);
    }

    [TestMethod]
    public async Task VideoWithoutCaptionsUsesSpeech()
    {
        captions.Result = null;
        speech.Result = new Transcript([new TranscriptSegment(TimeSpan.Zero, "spoken words")]);
        await AddPendingAsync("video");

        await RunAsync();

        Assert.AreSame(speech.Result, writer.LastTranscript);
    }

    [TestMethod]
    public async Task EmptyCaptionsCountAsNoCaptions()
    {
        captions.Result = new Transcript([]);
        speech.Result = new Transcript([new TranscriptSegment(TimeSpan.Zero, "spoken words")]);
        await AddPendingAsync("video");

        await RunAsync();

        Assert.AreSame(speech.Result, writer.LastTranscript);
    }

    [TestMethod]
    public async Task NoCaptionsAndNoSpeechKeepsVideoPendingWithTheReason()
    {
        captions.Result = null;
        await AddPendingAsync("video");

        await RunAsync();

        Assert.AreEqual(VideoState.Pending, videos.Find("video").State);
        Assert.AreEqual("speech unavailable", videos.Find("video").LastReason);
    }

    [TestMethod]
    public async Task EntryIsStoredEvenWithoutAnyImages()
    {
        images.Images = [];
        await AddPendingAsync("video");

        await RunAsync();

        Assert.HasCount(1, entries.Added);
    }

    [TestMethod]
    public async Task CancellingStopsTheRunWithoutCountingAFailure()
    {
        await AddPendingAsync("video");
        using var source = new CancellationTokenSource();
        await source.CancelAsync();
        var cancelled = new CancellingWriter();
        sut = CreateSut(limit: null, cancelled);

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.ExecuteAsync(_ => { }, source.Token));

        Assert.AreEqual(0, videos.Find("video").Attempts, "Stopping the application is not the video's fault.");
        Assert.IsEmpty(log.Runs);
    }

    [TestMethod]
    public async Task RunLimitLeavesTheRestPending()
    {
        await AddPendingAsync("one");
        await AddPendingAsync("two");
        await AddPendingAsync("three");
        sut = CreateSut(limit: 2);

        await RunAsync();

        Assert.HasCount(2, entries.Added);
        Assert.AreEqual(1, videos.Videos.Count(video => video.State == VideoState.Pending));
    }

    [TestMethod]
    public async Task ChannelProblemsAppearInTheRunRecord()
    {
        catalog.FailingChannels.Add(1);

        await RunAsync();

        StringAssert.Contains(log.Runs[0].Problem!, "Channel One");
    }

    [TestMethod]
    public async Task ProgressCountsVideos()
    {
        await AddPendingAsync("one");
        await AddPendingAsync("two");
        var reported = new List<RefreshProgress>();

        await sut.ExecuteAsync(reported.Add, CancellationToken.None);

        var last = reported[^1];
        Assert.AreEqual("Title of two", last.Activity);
        Assert.AreEqual(1, last.VideosDone);
        Assert.AreEqual(2, last.VideosTotal);
    }

    //

    private FakeTimeProvider time = null!;
    private FakeChannelStore channels = null!;
    private FakeVideoStore videos = null!;
    private FakeCatalog catalog = null!;
    private FakeRefreshLog log = null!;
    private FakeSetupMonitor setupMonitor = null!;
    private FakeCaptionSource captions = null!;
    private FakeSpeechTranscriber speech = null!;
    private FakeBriefingWriter writer = null!;
    private FakeWebSearch search = null!;
    private FakeImageSource images = null!;
    private FakeEntryStore entries = null!;
    private RefreshRunner sut = null!;

    private sealed class CancellingWriter : YouNews.Core.Ports.IBriefingWriter
    {
        public Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("The token should already be cancelled.");
        }

        public Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private RefreshRunner CreateSut(int? limit, YouNews.Core.Ports.IBriefingWriter? replacementWriter = null)
    {
        var discovery = new VideoDiscovery(channels, videos, catalog, time);
        var processor = new VideoProcessor(new VideoProcessorDependencies(captions, speech, replacementWriter ?? writer, search, images, entries, time));
        var dependencies = new RefreshDependencies(channels, videos, new FakeInterestStore(), log, setupMonitor, time);
        return new RefreshRunner(dependencies, discovery, processor, new RefreshLimits(limit));
    }

    private Task AddPendingAsync(string id)
    {
        return videos.AddAsync(new NewVideo(1, id, "Title of " + id, VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
    }

    private Task RunAsync() => sut.ExecuteAsync(_ => { }, CancellationToken.None);
}