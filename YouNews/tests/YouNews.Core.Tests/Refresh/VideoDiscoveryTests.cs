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
        channels.Channels.Add(new Channel(1, "UC1", "https://youtube.com/@one", "One"));
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

        await sut.DiscoverAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "recent", "edge" }, videos.Videos.Select(video => video.YouTubeId).ToArray());
        CollectionAssert.DoesNotContain(catalog.DetailRequests, "older", "Walking must stop at the first video older than the cutoff.");
    }

    [TestMethod]
    public async Task StopsAtAKnownVideoWithoutFetchingItsDetails()
    {
        await videos.AddAsync(new NewVideo(1, "known", "Known", VideoState.Done, null, time.GetUtcNow()), CancellationToken.None);
        SetFeed("new", "known", "behind");
        AddDetails("new", new DateOnly(2026, 10, 31));

        await sut.DiscoverAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "new" }, catalog.DetailRequests);
    }

    [TestMethod]
    [DataRow(VideoAvailability.Eligible, VideoState.Pending)]
    [DataRow(VideoAvailability.RecheckLater, VideoState.Recheck)]
    [DataRow(VideoAvailability.PermanentlyUnavailable, VideoState.Skipped)]
    public async Task InitialStateFollowsAvailability(VideoAvailability availability, VideoState expected)
    {
        SetFeed("video");
        catalog.Details["video"] = new VideoDetails("video", "Title", availability, null, "reason");

        await sut.DiscoverAsync(CancellationToken.None);

        Assert.AreEqual(expected, videos.Find("video").State);
    }

    [TestMethod]
    public async Task VideoWithoutDateDoesNotEndTheWalk()
    {
        SetFeed("members-only", "normal");
        catalog.Details["members-only"] = new VideoDetails("members-only", "members-only", VideoAvailability.PermanentlyUnavailable, null, "members only");
        AddDetails("normal", new DateOnly(2026, 10, 31));

        await sut.DiscoverAsync(CancellationToken.None);

        Assert.AreEqual(VideoState.Pending, videos.Find("normal").State, "A members-only video at the top must not hide the videos behind it.");
    }

    [TestMethod]
    public async Task AVideoInBothFeedsIsRecordedOnce()
    {
        catalog.Feeds[1] = [["same"], ["same", "stream"]];
        AddDetails("same", new DateOnly(2026, 10, 31));
        AddDetails("stream", new DateOnly(2026, 10, 30));

        await sut.DiscoverAsync(CancellationToken.None);

        Assert.AreEqual(1, videos.Videos.Count(video => video.YouTubeId == "same"));
    }

    [TestMethod]
    public async Task FailingChannelIsReportedAndOthersStillChecked()
    {
        channels.Channels.Add(new Channel(2, "UC2", "https://youtube.com/@two", "Two"));
        catalog.FailingChannels.Add(1);
        catalog.Feeds[2] = [["video"]];
        AddDetails("video", new DateOnly(2026, 10, 31));

        var problems = await sut.DiscoverAsync(CancellationToken.None);

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

        await sut.DiscoverAsync(CancellationToken.None);

        Assert.AreEqual(VideoState.Pending, videos.Find("stream").State);
    }

    [TestMethod]
    public async Task RecheckVideoIsGivenUpAfterSevenDays()
    {
        await videos.AddAsync(new NewVideo(1, "stream", "Stream", VideoState.Recheck, "is live", time.GetUtcNow()), CancellationToken.None);
        catalog.Feeds[1] = [[]];
        catalog.Details["stream"] = new VideoDetails("stream", "Stream", VideoAvailability.RecheckLater, null, "still live");

        time.Advance(TimeSpan.FromDays(6));
        await sut.DiscoverAsync(CancellationToken.None);
        Assert.AreEqual(VideoState.Recheck, videos.Find("stream").State, "Six days is inside the window.");

        time.Advance(TimeSpan.FromDays(2));
        await sut.DiscoverAsync(CancellationToken.None);
        Assert.AreEqual(VideoState.GaveUp, videos.Find("stream").State);
        Assert.AreEqual("still live", videos.Find("stream").LastReason);
    }

    //

    private FakeTimeProvider time = null!;
    private FakeChannelStore channels = null!;
    private FakeVideoStore videos = null!;
    private FakeCatalog catalog = null!;
    private VideoDiscovery sut = null!;

    private void SetFeed(params string[] ids) => catalog.Feeds[1] = [ids];

    private void AddDetails(string id, DateOnly publishedOn)
    {
        catalog.Details[id] = new VideoDetails(id, "Title of " + id, VideoAvailability.Eligible, publishedOn, null);
    }
}