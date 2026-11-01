using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class VideoDiscoveryTests
{
    [TestInitialize]
    public void Initialize()
    {
        time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero));
        time.SetLocalTimeZone(TimeZoneInfo.Utc);
        channels = new FakeChannelStore();
        channels.Channels.Add(new Channel(1, "https://youtube.com/@one", "One"));
        videos = new FakeVideoStore();
        catalog = new FakeCatalog();
        sut = new VideoDiscovery(channels, videos, catalog, time);
    }

    [TestMethod]
    public async Task NewChannelGoesBackOneMonthAndStopsAtOlderVideos()
    {
        SetFeed("recent", "edge", "old", "older");
        AddDetails("recent", new DateOnly(2026, 10, 30));
        AddDetails("edge", new DateOnly(2026, 10, 1));
        AddDetails("old", new DateOnly(2026, 9, 30));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "recent", "edge" }, videos.Videos.Select(video => video.YouTubeId).ToArray());
        CollectionAssert.DoesNotContain(catalog.DetailRequests, "older", "Walking must stop at the first video older than the cutoff.");
    }

    [TestMethod]
    public async Task DoesNotFetchTheDetailsOfAKnownVideo()
    {
        await videos.AddAsync(new NewVideo(1, "known", "Known", VideoState.Done, null, time.GetUtcNow()), CancellationToken.None);
        SetFeed("new", "known", "older");
        AddDetails("new", new DateOnly(2026, 10, 31));
        AddDetails("older", new DateOnly(2026, 9, 1));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "new", "older" }, catalog.DetailRequests, "The walk goes on to the cutoff, but must not ask about the known video.");
    }

    [TestMethod]
    public async Task FindsOlderVideosLeftBehindByAnInterruptedRun()
    {
        await videos.AddAsync(new NewVideo(1, "reached", "Reached before stopping", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
        catalog.ListedFeeds[1] = [[Listed("reached", new DateOnly(2026, 10, 30)), Listed("left-behind", new DateOnly(2026, 10, 20))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(VideoState.Pending, videos.Find("left-behind").State);
    }

    [TestMethod]
    public async Task ARecentListedVideoNeedsNoDetailsCall()
    {
        catalog.ListedFeeds[1] = [[Listed("recent", new DateOnly(2026, 10, 25))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.IsEmpty(catalog.DetailRequests);
        Assert.AreEqual("Listed recent", videos.Find("recent").Title);
        Assert.AreEqual(new DateOnly(2026, 10, 25), videos.PublishedOn["recent"]);
    }

    [TestMethod]
    public async Task AListedVideoOlderThanTheCutoffEndsTheWalkWithoutADetailsCall()
    {
        catalog.ListedFeeds[1] = [[Listed("old", new DateOnly(2026, 9, 2)), Listed("older", new DateOnly(2026, 8, 2))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.IsEmpty(videos.Videos);
        Assert.IsEmpty(catalog.DetailRequests);
    }

    [TestMethod]
    public async Task ARoundedListedDateIsConfirmedAndTheRealDateEndsTheWalk()
    {
        catalog.ListedFeeds[1] = [[Listed("rounded", new DateOnly(2026, 10, 2)), Listed("older", new DateOnly(2026, 10, 2))]];
        AddDetails("rounded", new DateOnly(2026, 9, 10));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.IsEmpty(videos.Videos, "YouTube says \"1 month ago\" up to two months back, so the listed date alone would let this video in.");
        CollectionAssert.AreEqual(new[] { "rounded" }, catalog.DetailRequests);
    }

    [TestMethod]
    public async Task AKnownVideoDoesNotGetARoundedPublicationDateFromTheListing()
    {
        await videos.AddAsync(new NewVideo(1, "known", "Known", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
        catalog.ListedFeeds[1] = [[Listed("known", new DateOnly(2026, 10, 2))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.IsFalse(videos.PublishedOn.ContainsKey("known"), "\"1 month ago\" can mean up to two months, and a stored date is never corrected.");
    }

    [TestMethod]
    public async Task AKnownVideoGetsItsMissingPublicationDateFromTheListing()
    {
        await videos.AddAsync(new NewVideo(1, "known", "Known", VideoState.Pending, null, time.GetUtcNow()), CancellationToken.None);
        catalog.ListedFeeds[1] = [[Listed("known", new DateOnly(2026, 10, 30))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(new DateOnly(2026, 10, 30), videos.PublishedOn["known"]);
        Assert.IsEmpty(catalog.DetailRequests);
    }

    [TestMethod]
    public async Task AMembersOnlyListedVideoIsSkippedWithoutADetailsCall()
    {
        var details = new VideoDetails("Members", VideoAvailability.PermanentlyUnavailable, new DateOnly(2026, 10, 25), "members only");
        catalog.ListedFeeds[1] = [[new FeedVideo("members", details)]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(VideoState.Skipped, videos.Find("members").State);
        Assert.AreEqual(0, listener.VideosQueued, "A skipped video gives the consumer nothing to do.");
        Assert.IsEmpty(catalog.DetailRequests);
    }

    [TestMethod]
    public async Task TellsTheListenerAboutChannelsAndQueuedVideos()
    {
        channels.Channels.Add(new Channel(2, "https://youtube.com/@two", "Two"));
        await videos.AddAsync(new NewVideo(1, "stream", "Stream", VideoState.Recheck, "is live", time.GetUtcNow()), CancellationToken.None);
        AddDetails("stream", new DateOnly(2026, 10, 31));
        catalog.ListedFeeds[1] = [[Listed("first", new DateOnly(2026, 10, 30))]];
        catalog.ListedFeeds[2] = [[Listed("second", new DateOnly(2026, 10, 30))]];

        await sut.DiscoverAsync(listener, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "1/2 One", "2/2 Two" }, listener.Channels);
        Assert.AreEqual(3, listener.VideosQueued, "Two new videos and the finished stream.");
    }

    [TestMethod]
    [DataRow(VideoAvailability.Eligible, VideoState.Pending)]
    [DataRow(VideoAvailability.RecheckLater, VideoState.Recheck)]
    [DataRow(VideoAvailability.PermanentlyUnavailable, VideoState.Skipped)]
    public async Task InitialStateFollowsAvailability(VideoAvailability availability, VideoState expected)
    {
        SetFeed("video");
        catalog.Details["video"] = new VideoDetails("Title", availability, null, "reason");

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(expected, videos.Find("video").State);
    }

    [TestMethod]
    public async Task VideoWithoutDateDoesNotEndTheWalk()
    {
        SetFeed("members-only", "normal");
        catalog.Details["members-only"] = new VideoDetails("members-only", VideoAvailability.PermanentlyUnavailable, null, "members only");
        AddDetails("normal", new DateOnly(2026, 10, 31));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(VideoState.Pending, videos.Find("normal").State, "A members-only video at the top must not hide the videos behind it.");
    }

    [TestMethod]
    public async Task AVideoInBothFeedsIsRecordedOnce()
    {
        catalog.Feeds[1] = [["same"], ["same", "stream"]];
        AddDetails("same", new DateOnly(2026, 10, 31));
        AddDetails("stream", new DateOnly(2026, 10, 30));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(1, videos.Videos.Count(video => video.YouTubeId == "same"));
    }

    [TestMethod]
    public async Task FailingChannelIsReportedAndOthersStillChecked()
    {
        channels.Channels.Add(new Channel(2, "https://youtube.com/@two", "Two"));
        catalog.FailingChannels.Add(1);
        catalog.Feeds[2] = [["video"]];
        AddDetails("video", new DateOnly(2026, 10, 31));

        var problems = await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.HasCount(1, problems);
        StringAssert.Contains(problems[0], "One");
        Assert.AreEqual(VideoState.Pending, videos.Find("video").State);
    }

    [TestMethod]
    public async Task RecheckVideoBecomesPendingWhenItIsFinished()
    {
        await videos.AddAsync(new NewVideo(1, "stream", "Stream", VideoState.Recheck, "is live", time.GetUtcNow()), CancellationToken.None);
        catalog.Feeds[1] = [[]];
        AddDetails("stream", new DateOnly(2026, 10, 31));

        await sut.DiscoverAsync(listener, CancellationToken.None);

        Assert.AreEqual(VideoState.Pending, videos.Find("stream").State);
    }

    [TestMethod]
    public async Task RecheckVideoIsGivenUpAfterSevenDays()
    {
        await videos.AddAsync(new NewVideo(1, "stream", "Stream", VideoState.Recheck, "is live", time.GetUtcNow()), CancellationToken.None);
        catalog.Feeds[1] = [[]];
        catalog.Details["stream"] = new VideoDetails("Stream", VideoAvailability.RecheckLater, null, "still live");

        time.Advance(TimeSpan.FromDays(6));
        await sut.DiscoverAsync(listener, CancellationToken.None);
        Assert.AreEqual(VideoState.Recheck, videos.Find("stream").State, "Six days is inside the window.");

        time.Advance(TimeSpan.FromDays(2));
        await sut.DiscoverAsync(listener, CancellationToken.None);
        Assert.AreEqual(VideoState.GaveUp, videos.Find("stream").State);
        Assert.AreEqual("still live", videos.Find("stream").LastReason);
    }

    [TestMethod]
    public async Task AVideoThatFailsItsRecheckDoesNotBlockTheChannels()
    {
        await videos.AddAsync(new NewVideo(1, "broken", "Broken video", VideoState.Recheck, "is live", time.GetUtcNow()), CancellationToken.None);
        SetFeed("new");
        AddDetails("new", new DateOnly(2026, 10, 31));

        var problems = await sut.DiscoverAsync(listener, CancellationToken.None);

        StringAssert.Contains(problems.Single(), "Broken video");
        Assert.AreEqual(VideoState.Recheck, videos.Find("broken").State, "It must be tried again on the next run.");
        Assert.AreEqual(VideoState.Pending, videos.Find("new").State, "The channel must still be checked.");
    }

    //

    private FakeTimeProvider time = null!;
    private FakeChannelStore channels = null!;
    private FakeVideoStore videos = null!;
    private FakeCatalog catalog = null!;
    private FakeDiscoveryListener listener = new();
    private VideoDiscovery sut = null!;

    private static FeedVideo Listed(string id, DateOnly publishedOn) =>
        new(id, new VideoDetails("Listed " + id, VideoAvailability.Eligible, publishedOn, null));

    private void SetFeed(params string[] ids) => catalog.Feeds[1] = [ids];

    private void AddDetails(string id, DateOnly publishedOn)
    {
        catalog.Details[id] = new VideoDetails("Title of " + id, VideoAvailability.Eligible, publishedOn, null);
    }
}