using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
using YouNews.Core.Tests.Fakes;

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
        channels.Channels.Add(new Channel(1, "https://youtube.com/@one", "Channel One"));
        videos = new FakeVideoStore();
        catalog = new FakeCatalog
        {
            UnknownVideosHaveNoDate = true,
            Feeds =
            {
                [1] = [[]],
            },
        };
        log = new FakeRefreshLog();
        alerts = new FakeRunAlertStore();
        setupMonitor = new FakeSetupMonitor();
        captions = new FakeCaptionSource();
        speech = new FakeSpeechTranscriber();
        writer = new FakeBriefingWriter();
        search = new FakeWebSearch();
        images = new FakeImageSource();
        entries = new FakeEntryStore
        {
            OnAdded = videoId => videos.SetStateAsync(videoId, VideoState.Done, null, CancellationToken.None),
        };
        sut = CreateSut(limit: null);
    }

    [TestMethod]
    public async Task ProducesAnEntryDatedByRetrievalWhenThePublicationDateIsUnknown()
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
        CollectionAssert.AreEqual(new[] { "query one" }, search.Queries, "The search must use the queries from the analysis.");
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

    [TestMethod]
    public async Task AVideoIsProcessedWhileDiscoveryIsStillRunning()
    {
        var discoveryMayEnd = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var discovery = new ScriptedDiscovery(async listener =>
        {
            await videos.AddAsync(new NewVideo(1, "early", "Title of early", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
            listener.VideoQueued();
            await discoveryMayEnd.Task.WaitAsync(TimeSpan.FromSeconds(10));
        });
        entries.OnAdded = videoId =>
        {
            videos.SetStateAsync(videoId, VideoState.Done, null, CancellationToken.None);
            discoveryMayEnd.TrySetResult();
        };
        sut = CreateSut(limit: null, discovery: discovery);

        await RunAsync();

        Assert.HasCount(1, entries.Added, "Discovery only ends once the video is done, so a run that waits for discovery first would time out.");
        Assert.IsNull(log.Runs.Single().Problem, log.Runs.Single().Problem);
    }

    [TestMethod]
    public async Task AVideoFoundWhileTheOthersAreBeingProcessedIsTakenInStoreOrder()
    {
        await AddPendingAsync("waiting");
        var discovery = new ScriptedDiscovery(_ => Task.CompletedTask);
        sut = CreateSut(limit: null, discovery: discovery);
        entries.OnAdded = videoId =>
        {
            videos.SetStateAsync(videoId, VideoState.Done, null, CancellationToken.None);
            if (entries.Added.Count == 1)
                videos.AddAsync(new NewVideo(1, "late", "Title of late", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
        };

        await RunAsync();

        Assert.HasCount(2, entries.Added, "The store is asked again before every video, so the late one must not be missed.");
    }

    [TestMethod]
    public async Task ConsumerWaitsForDiscoveryWhenTheQueueIsEmpty()
    {
        var discovery = new ScriptedDiscovery(async listener =>
        {
            await Task.Delay(50);
            await videos.AddAsync(new NewVideo(1, "slow", "Title of slow", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
            listener.VideoQueued();
        });
        sut = CreateSut(limit: null, discovery: discovery);

        await RunAsync();

        Assert.AreEqual(VideoState.Done, videos.Find("slow").State);
    }

    [TestMethod]
    public async Task AFailedDiscoveryIsRecordedAndWaitingVideosStillRun()
    {
        await AddPendingAsync("video");
        sut = CreateSut(limit: null, discovery: new ScriptedDiscovery(_ => throw new InvalidOperationException("no channels today")));

        await RunAsync();

        Assert.AreEqual(VideoState.Done, videos.Find("video").State);
        StringAssert.Contains(log.Runs.Single().Problem!, "no channels today");
    }

    [TestMethod]
    public async Task StoppingEndsBothSidesAndRecordsNoRun()
    {
        using var source = new CancellationTokenSource();
        var discoverySawStop = false;
        var discovery = new ScriptedDiscovery(async _ =>
        {
            // ReSharper disable AccessToDisposedClosure
            await source.CancelAsync();
            await Task.Delay(Timeout.Infinite, source.Token).ContinueWith(_ => discoverySawStop = true, TaskScheduler.Default);
            source.Token.ThrowIfCancellationRequested();
            // ReSharper restore AccessToDisposedClosure
        });
        sut = CreateSut(limit: null, discovery: discovery);

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.ExecuteAsync(_ => { }, source.Token));

        Assert.IsTrue(discoverySawStop);
        Assert.IsEmpty(log.Runs);
    }

    [TestMethod]
    public async Task ProgressShowsDiscoveryAlongsideTheVideoAndClearsItAtTheEnd()
    {
        var reported = new List<RefreshProgress>();
        var discovery = new ScriptedDiscovery(listener =>
        {
            listener.ChannelStarted(3, 23, "Locally");
            listener.VideoQueued();
            return Task.CompletedTask;
        });
        sut = CreateSut(limit: null, discovery: discovery);

        await sut.ExecuteAsync(reported.Add, CancellationToken.None);

        Assert.IsTrue(reported.Exists(progress => progress.Discovery == "Checking channel 3 of 23 (Locally), 1 new so far"), string.Join(" | ", reported));
        Assert.IsNull(reported[^1].Discovery);
    }

    [TestMethod]
    public async Task EntryWithoutAPublicationDateIsFiledUnderTheDayTheVideoWasDiscovered()
    {
        await AddPendingAsync("video");
        time.Advance(TimeSpan.FromDays(1));

        await RunAsync();

        Assert.AreEqual(new DateOnly(2026, 11, 2), entries.Added.Single().Day, "A backlog must not move a video to the day it finally got processed.");
    }

    [TestMethod]
    public async Task EntryIsFiledUnderTheExactPublicationDateRatherThanTheRoundedListingDate()
    {
        await videos.AddAsync(new NewVideo(1, "video", "Title of video", VideoState.Pending, null, time.GetUtcNow(), new DateOnly(2026, 10, 19)), CancellationToken.None);
        catalog.Details["video"] = new VideoDetails("Title of video", VideoAvailability.Eligible, new DateOnly(2026, 10, 14), null);

        await RunAsync();

        Assert.AreEqual(new DateOnly(2026, 10, 14), entries.Added.Single().Day, "The video page's date wins over the stored listing date and the discovery day.");
    }

    [TestMethod]
    public async Task EntryFallsBackToTheStoredDateWhenTheExactDateCannotBeRead()
    {
        await videos.AddAsync(new NewVideo(1, "video", "Title of video", VideoState.Pending, null, time.GetUtcNow(), new DateOnly(2026, 10, 19)), CancellationToken.None);
        catalog.Details["video"] = new VideoDetails("video", VideoAvailability.RecheckLater, null, "yt-dlp timed out.");

        await RunAsync();

        Assert.AreEqual(new DateOnly(2026, 10, 19), entries.Added.Single().Day, "A failed lookup must not move the video to the day it was discovered.");
    }

    [TestMethod]
    public async Task AStoreFailureWhileRecordingAFailureDoesNotEndTheRun()
    {
        await AddPendingAsync("bad");
        await AddPendingAsync("good");
        writer.FailingTitles.Add("Title of bad");
        videos.FailRecordingFailures = true;

        await RunAsync();

        Assert.AreEqual(VideoState.Done, videos.Find("good").State);
        Assert.AreEqual(1, log.Runs.Single().Completed);
        Assert.AreEqual(1, log.Runs.Single().Failed);
    }

    [TestMethod]
    public async Task RateLimitedVideoGoesToTheEndOfTheQueueAndTheRunCarriesOnAfterAPause()
    {
        await AddPendingAsync("first");
        await AddPendingAsync("limited");
        await AddPendingAsync("third");
        captions.RateLimitedIds.Add("limited");
        var reported = new List<RefreshProgress>();

        await sut.ExecuteAsync(progress => { reported.Add(progress); LiftRateLimitsWhenPaused(progress); }, CancellationToken.None);

        var order = entries.Added.Select(entry => videos.Videos.Single(video => video.Id == entry.VideoId).YouTubeId).ToArray();
        CollectionAssert.AreEqual(new[] { "first", "third", "limited" }, order, string.Join(", ", order));
        Assert.AreEqual(0, videos.Find("limited").Attempts, "Being rate-limited is not the video's fault.");
        Assert.IsTrue(
            reported.Exists(progress => progress.Activity == "YouTube's rate limit was reached, next video at 01:40"),
            "23:30 UTC plus 10 minutes is 01:40 in the +2 zone. Reported: " + string.Join(" | ", reported.Select(progress => progress.Activity)));
        var run = log.Runs.Single();
        Assert.AreEqual(3, run.Completed);
        Assert.AreEqual(0, run.Failed);
        StringAssert.Contains(run.Problem!, "rate limit paused the run once", run.Problem);
    }

    [TestMethod]
    public async Task RunWaitsTenMinutesBeforeTheNextVideoAfterARateLimit()
    {
        await AddPendingAsync("limited");
        await AddPendingAsync("next");
        captions.RateLimitedIds.Add("limited");
        var paused = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = sut.ExecuteAsync(
            progress =>
            {
                if (progress.Activity.StartsWith("YouTube's rate limit", StringComparison.Ordinal))
                    paused.TrySetResult();
            },
            CancellationToken.None);

        await paused.Task.WaitAsync(TimeSpan.FromSeconds(10));
        time.Advance(RefreshRunner.RATE_LIMIT_PAUSE - TimeSpan.FromSeconds(1));
        await Task.Delay(100);
        Assert.IsEmpty(entries.Added, "No video may start before the pause is over.");

        captions.RateLimitedIds.Clear();
        time.Advance(TimeSpan.FromSeconds(1));
        await running.WaitAsync(TimeSpan.FromSeconds(10));

        Assert.HasCount(2, entries.Added);
    }

    [TestMethod]
    public async Task RateLimitProblemIsRecordedAlongsideChannelProblems()
    {
        catalog.FailingChannels.Add(1);
        await AddPendingAsync("limited");
        captions.RateLimitedIds.Add("limited");

        await RunAsync();

        var problem = log.Runs.Single().Problem!;
        StringAssert.Contains(problem, "Channel One", problem);
        StringAssert.Contains(problem, "rate limit", problem);
    }

    [TestMethod]
    public async Task FiveRefusalsInARowStopTheRunWithoutAFifthWaitAndRaiseAnAlert_VideoSide()
    {
        var start = time.GetUtcNow();
        await AddPendingAsync("limited");
        captions.RateLimitedIds.Add("limited");
        var searching = new DiscoveryThatWaitsForTheStop();
        sut = CreateSut(limit: null, discovery: searching);

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        Assert.AreEqual(4 * RefreshRunner.RATE_LIMIT_PAUSE, time.GetUtcNow() - start, "Four pauses, and none after the fifth refusal.");
        Assert.IsTrue(searching.WasStopped, "The search for new videos must be cancelled when the run stops.");
        var run = log.Runs.Single();
        StringAssert.Contains(run.Problem!, "refused 5 requests in a row", run.Problem);
        StringAssert.Contains(run.Problem!, "paused the run 4 times", run.Problem);
        Assert.AreEqual(VideoState.Pending, videos.Find("limited").State);
        Assert.AreEqual(1, alerts.Raised);
        StringAssert.Contains(alerts.Current!.Message, "refused 5 requests in a row", alerts.Current.Message);
        StringAssert.Contains(alerts.Current.Message, "The last refusal was: HTTP Error 429: Too Many Requests for limited", alerts.Current.Message);
        StringAssert.Contains(run.Problem!, "The last refusal was: HTTP Error 429: Too Many Requests for limited", run.Problem);
        Assert.AreEqual(run.FinishedAt, alerts.Current.RaisedAt);
    }

    [TestMethod]
    public async Task FiveRefusalsInARowStopTheRun_ChannelSideCountsToo()
    {
        var start = time.GetUtcNow();
        catalog.RateLimitedChannels.Add(1);
        catalog.Feeds[1] = [[]];

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        Assert.AreEqual(4 * RefreshRunner.RATE_LIMIT_PAUSE, time.GetUtcNow() - start, "Four pauses, and none after the fifth refusal.");
        StringAssert.Contains(log.Runs.Single().Problem!, "refused 5 requests in a row", log.Runs.Single().Problem);
        Assert.AreEqual(1, alerts.Raised);
        StringAssert.Contains(alerts.Current!.Message, "The last refusal was: HTTP Error 429: Too Many Requests for channel 1", alerts.Current.Message);
    }

    [TestMethod]
    public async Task FiveRefusalsInARowStopTheRun_SplitBetweenBothSides()
    {
        await AddPendingAsync("limited");
        captions.RateLimitedIds.Add("limited");
        sut = CreateSut(limit: null, discovery: new RefusedDiscovery());

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        var problem = log.Runs.Single().Problem!;
        StringAssert.Contains(problem, "paused the run 4 times", "Both sides share one count of four waits. " + problem);
        StringAssert.Contains(problem, "refused 5 requests in a row", problem);
        Assert.AreEqual(1, alerts.Raised);
    }

    [TestMethod]
    public async Task ASuccessfulVideoBetweenRefusalsRestartsTheCount_FourRefusalsOneSuccessFourRefusalsDoNotStop()
    {
        await AddPendingAsync("a");
        await AddPendingAsync("b");
        var scripted = new SequencedCaptionSource([true, true, true, true, false, true, true, true, true, false]);
        sut = CreateSut(limit: null, captionSource: scripted);

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        var run = log.Runs.Single();
        Assert.AreEqual(2, run.Completed, run.Problem);
        Assert.AreEqual(0, alerts.Raised, run.Problem);
        Assert.DoesNotContain("requests in a row", run.Problem!, run.Problem);
        Assert.AreEqual(10, scripted.Calls, "Every scripted answer should have been asked for.");
    }

    [TestMethod]
    public async Task AFailedVideoBetweenRefusalsAlsoRestartsTheCount()
    {
        await AddPendingAsync("a");
        await AddPendingAsync("b");
        writer.FailingTitles.Add("Title of a");
        var scripted = new SequencedCaptionSource([true, true, true, true, false, true, true, true, true, false]);
        sut = CreateSut(limit: null, captionSource: scripted);

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        var run = log.Runs.Single();
        Assert.AreEqual(0, alerts.Raised, run.Problem);
        Assert.AreEqual(1, run.Failed, run.Problem);
    }

    [TestMethod]
    public async Task AcceptedChannelCheckBetweenRefusalsRestartsTheCount()
    {
        sut = CreateSut(limit: null, discovery: new ScriptedDiscovery(async listener =>
        {
            for (var round = 0; round < 2; round++)
            {
                for (var refusal = 0; refusal < 4; refusal++)
                    await listener.WaitOutChannelRateLimitAsync("HTTP Error 429: Too Many Requests", CancellationToken.None);
                listener.ChannelChecked();
            }
        }));

        await sut.ExecuteAsync(PassPauses, CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(10));

        Assert.AreEqual(0, alerts.Raised, log.Runs.Single().Problem);
        StringAssert.Contains(log.Runs.Single().Problem!, "paused the run 8 times", log.Runs.Single().Problem);
    }

    [TestMethod]
    public async Task AlertStaysWhenALaterRunGoesWell()
    {
        await alerts.RaiseAsync("old alert", time.GetUtcNow(), CancellationToken.None);

        await RunAsync();

        Assert.AreEqual("old alert", alerts.Current?.Message, "Only dismissing may clear the alert.");
    }

    [TestMethod]
    public async Task VideoIsGivenUpAfterTooManyFailedAttempts()
    {
        await AddPendingAsync("bad");
        writer.FailingTitles.Add("Title of bad");

        for (var run = 1; run < RefreshRunner.MAXIMUM_ATTEMPTS; run++)
            await RunAsync();

        Assert.AreEqual(VideoState.Pending, videos.Find("bad").State, $"Still retried after {RefreshRunner.MAXIMUM_ATTEMPTS - 1} failures.");

        await RunAsync();

        var bad = videos.Find("bad");
        Assert.AreEqual(VideoState.GaveUp, bad.State);
        Assert.AreEqual(RefreshRunner.MAXIMUM_ATTEMPTS, bad.Attempts);
        Assert.AreEqual("model is down", bad.LastReason);
    }

    //

    private FakeTimeProvider time = null!;
    private FakeChannelStore channels = null!;
    private FakeVideoStore videos = null!;
    private FakeCatalog catalog = null!;
    private FakeRefreshLog log = null!;
    private FakeRunAlertStore alerts = null!;
    private FakeSetupMonitor setupMonitor = null!;
    private FakeCaptionSource captions = null!;
    private FakeSpeechTranscriber speech = null!;
    private FakeBriefingWriter writer = null!;
    private FakeWebSearch search = null!;
    private FakeImageSource images = null!;
    private FakeEntryStore entries = null!;
    private RefreshRunner sut = null!;
    private readonly HashSet<string> announcedPauses = [];

    private sealed class ScriptedDiscovery(Func<IDiscoveryListener, Task> script) : IVideoDiscovery
    {
        public async Task<IReadOnlyList<string>> DiscoverAsync(IDiscoveryListener listener, CancellationToken cancellationToken)
        {
            await script(listener);
            return [];
        }
    }

    /// <summary>A search that has nothing to do but wait, and so ends only when the run stops it.</summary>
    private sealed class DiscoveryThatWaitsForTheStop : IVideoDiscovery
    {
        public bool WasStopped { get; private set; }

        public async Task<IReadOnlyList<string>> DiscoverAsync(IDiscoveryListener listener, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken).ContinueWith(_ => WasStopped = cancellationToken.IsCancellationRequested, TaskScheduler.Default);
            return [];
        }
    }

    /// <summary>A search that finds YouTube refusing every channel check, until the run stops it.</summary>
    private sealed class RefusedDiscovery : IVideoDiscovery
    {
        public async Task<IReadOnlyList<string>> DiscoverAsync(IDiscoveryListener listener, CancellationToken cancellationToken)
        {
            while (true)
                await listener.WaitOutChannelRateLimitAsync("HTTP Error 429: Too Many Requests", cancellationToken);
        }
    }

    private sealed class SequencedCaptionSource(IEnumerable<bool> refusals) : ICaptionSource
    {
        public int Calls { get; private set; }

        public Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken)
        {
            Calls++;
            return answers.TryDequeue(out var refuse) && refuse
                ? throw new RateLimitedException("HTTP Error 429: Too Many Requests")
                : Task.FromResult<Transcript?>(new Transcript([new TranscriptSegment(TimeSpan.Zero, "words from captions")]));
        }

        private readonly Queue<bool> answers = new(refusals);
    }

    private sealed class CancellingWriter : IBriefingWriter
    {
        public Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("The token should already be cancelled.");
        }

        public Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private RefreshRunner CreateSut(int? limit, IBriefingWriter? replacementWriter = null, IVideoDiscovery? discovery = null, ICaptionSource? captionSource = null)
    {
        discovery ??= new VideoDiscovery(channels, videos, catalog, time);
        var processor = new VideoProcessor(new VideoProcessorDependencies(catalog, captionSource ?? captions, speech, replacementWriter ?? writer, search, images, entries, time));
        var dependencies = new RefreshDependencies(channels, videos, new FakeInterestStore(), log, alerts, setupMonitor, time);
        return new RefreshRunner(dependencies, discovery, processor, new RefreshLimits(limit));
    }

    private Task AddPendingAsync(string id) => videos.AddAsync(new NewVideo(1, id, "Title of " + id, VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);

    /// <summary>Runs with YouTube's rate limit lifting during every pause, as it does once YouTube is left alone for a while.</summary>
    private Task RunAsync() => sut.ExecuteAsync(LiftRateLimitsWhenPaused, CancellationToken.None);

    /// <summary>Ends each pause at once by moving the clock past it. Every pause announces a different resume time, so each is seen once.</summary>
    private void LiftRateLimitsWhenPaused(RefreshProgress progress)
    {
        if (!IsNewPause(progress))
            return;

        captions.RateLimitedIds.Clear();
        catalog.RateLimitedChannels.Clear();
        time.Advance(RefreshRunner.RATE_LIMIT_PAUSE);
    }

    /// <summary>Ends each pause at once by moving the clock past it, while YouTube keeps refusing.</summary>
    private void PassPauses(RefreshProgress progress)
    {
        if (IsNewPause(progress))
            time.Advance(RefreshRunner.RATE_LIMIT_PAUSE);
    }

    private bool IsNewPause(RefreshProgress progress)
    {
        var pause = new[] { progress.Activity, progress.Discovery }.FirstOrDefault(text => text?.StartsWith("YouTube's rate limit", StringComparison.Ordinal) == true);
        return pause is not null && announcedPauses.Add(pause);
    }
}