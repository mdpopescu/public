using YouNews.Core.Model;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class YtDlpVideoCatalogTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        environment.Options.Tools.NodePath = @"C:\tools\node.exe";
        runner = new FakeProcessRunner();
        sut = new YtDlpVideoCatalog(new YtDlpRunner(runner, environment.Wrapped), new VideoErrorClassifier(), new ChannelAddressParser());
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task EveryCallPassesNoUpdateJsRuntimeAndNoWarningsWithConfiguredTimeout()
    {
        runner.Behavior = _ => new ProcessResult(0, DetailsJson("not_live"), "");

        await sut.GetDetailsAsync("zhnJo7s91ls", CancellationToken.None);

        var call = runner.Invocations.Single();
        CollectionAssert.AreEqual(new[] { "--no-update", "--js-runtimes", @"node:C:\tools\node.exe", "--no-warnings" }, call.Arguments.Take(4).ToArray(), call.ToString());
        Assert.AreEqual(TimeSpan.FromMinutes(7), call.Timeout);
        Assert.AreEqual("yt-dlp", call.FileName);
    }

    [TestMethod]
    [DataRow("not_live", VideoAvailability.Eligible)]
    [DataRow("was_live", VideoAvailability.Eligible)]
    [DataRow("is_live", VideoAvailability.RecheckLater)]
    [DataRow("is_upcoming", VideoAvailability.RecheckLater)]
    [DataRow("post_live", VideoAvailability.RecheckLater)]
    public async Task ClassifiesByLiveStatus(string liveStatus, VideoAvailability expected)
    {
        runner.Behavior = _ => new ProcessResult(0, DetailsJson(liveStatus), "");

        var details = await sut.GetDetailsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(expected, details.Availability, $"live_status {liveStatus}");
        Assert.AreEqual("Some title", details.Title);
        Assert.AreEqual(new DateOnly(2026, 10, 31), details.PublishedOn);
        Assert.AreEqual(expected == VideoAvailability.Eligible, details.Reason == null, $"reason: {details.Reason}");
    }

    [TestMethod]
    public async Task FallsBackToTimestampWhenUploadDateIsMissing()
    {
        runner.Behavior = _ => new ProcessResult(0, """{"title":"T","timestamp":1793491200,"live_status":"not_live"}""", "");

        var details = await sut.GetDetailsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(new DateOnly(2026, 11, 1), details.PublishedOn);
    }

    [TestMethod]
    public async Task MembersOnlyVideoIsPermanentlyUnavailableAndTitleIsTheId()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: [youtube] ZerTreaAgPA: Join this channel to get access to members-only content like this video\n");

        var details = await sut.GetDetailsAsync("ZerTreaAgPA", CancellationToken.None);

        Assert.AreEqual(VideoAvailability.PermanentlyUnavailable, details.Availability);
        Assert.AreEqual("ZerTreaAgPA", details.Title);
        Assert.IsNull(details.PublishedOn);
        StringAssert.Contains(details.Reason, "members-only");
    }

    [TestMethod]
    public async Task ToolTimeoutIsRecheckLater()
    {
        runner.Behavior = _ => throw new TimeoutException();

        var details = await sut.GetDetailsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(VideoAvailability.RecheckLater, details.Availability);
        Assert.IsNotNull(details.Reason);
    }

    [TestMethod]
    public async Task RejectsVideoIdThatCouldEscapeAFolder()
    {
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => sut.GetDetailsAsync(@"..\..\evil", CancellationToken.None));

        Assert.IsEmpty(runner.Invocations);
    }

    [TestMethod]
    public async Task ListsVideosAndStreamsAsSeparateFeedsInOrder()
    {
        runner.Behavior = call => new ProcessResult(0, call.Arguments[^1].EndsWith("/videos", StringComparison.Ordinal) ? "{\"id\":\"vid1\"}\n{\"id\":\"vid2\"}\n" : "{\"id\":\"live1\"}\r\n", "");

        var feeds = await sut.ListVideoFeedsAsync(Channel("https://www.youtube.com/@matthew_berman/"), CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "vid1", "vid2" }, feeds[0].Select(video => video.YouTubeId).ToArray());
        CollectionAssert.AreEqual(new[] { "live1" }, feeds[1].Select(video => video.YouTubeId).ToArray());
        Assert.AreEqual("https://www.youtube.com/@matthew_berman/videos", runner.Invocations[0].Arguments[^1]);
        Assert.AreEqual("https://www.youtube.com/@matthew_berman/streams", runner.Invocations[1].Arguments[^1]);
        CollectionAssert.Contains(runner.Invocations[0].Arguments.ToList(), "200");
    }

    [TestMethod]
    public async Task ListingGivesTitleDateAndAvailabilityOfEachVideo()
    {
        const string LISTING = """
            {"id":"normal","title":"Normal video","upload_date":"20261030"}
            {"id":"members","title":"Members video","upload_date":"20261029","availability":"subscriber_only"}
            {"id":"upcoming","title":"Upcoming video","live_status":"is_upcoming"}
            {"id":"past","title":"Past stream","upload_date":"20261025","live_status":"was_live"}
            """;
        runner.Behavior = call => new ProcessResult(0, call.Arguments[^1].EndsWith("/videos", StringComparison.Ordinal) ? LISTING : "", "");

        var feed = (await sut.ListVideoFeedsAsync(Channel("https://www.youtube.com/@x"), CancellationToken.None))[0];

        Assert.AreEqual(new VideoDetails("Normal video", VideoAvailability.Eligible, new DateOnly(2026, 10, 30), null), feed[0].Details);
        Assert.AreEqual(VideoAvailability.PermanentlyUnavailable, feed[1].Details.Availability);
        Assert.AreEqual("Members video", feed[1].Details.Title);
        Assert.AreEqual(VideoAvailability.RecheckLater, feed[2].Details.Availability);
        Assert.IsNull(feed[2].Details.PublishedOn);
        Assert.AreEqual(VideoAvailability.Eligible, feed[3].Details.Availability);
        CollectionAssert.Contains(runner.Invocations[0].Arguments.ToList(), "youtubetab:approximate_date");
    }

    [TestMethod]
    public async Task ListedUpcomingStreamWithNullTimestampHasNoPublishDate()
    {
        const string LISTING = """
            {"id":"upcoming","title":"Upcoming stream","duration":null,"timestamp":null,"upload_date":null,"live_status":"is_upcoming","availability":null}
            {"id":"normal","title":"Normal","timestamp":1793491200}
            """;
        runner.Behavior = call => new ProcessResult(0, call.Arguments[^1].EndsWith("/streams", StringComparison.Ordinal) ? LISTING : "{\"id\":\"vid1\"}\n", "");

        var feeds = await sut.ListVideoFeedsAsync(Channel("https://www.youtube.com/@x"), CancellationToken.None);

        Assert.AreEqual(VideoAvailability.RecheckLater, feeds[1][0].Details.Availability);
        Assert.IsNull(feeds[1][0].Details.PublishedOn);
        Assert.AreEqual(new DateOnly(2026, 11, 1), feeds[1][1].Details.PublishedOn);
    }

    [TestMethod]
    public async Task ChannelWithoutStreamsTabHasEmptySecondFeed()
    {
        runner.Behavior = call => call.Arguments[^1].EndsWith("/videos", StringComparison.Ordinal)
            ? new ProcessResult(0, "{\"id\":\"vid1\"}\n", "")
            : new ProcessResult(1, "", "ERROR: [youtube:tab] This channel does not have a streams tab");

        var feeds = await sut.ListVideoFeedsAsync(Channel("https://www.youtube.com/@x"), CancellationToken.None);

        Assert.HasCount(1, feeds[0]);
        Assert.IsEmpty(feeds[1]);
    }

    [TestMethod]
    public async Task VideosTabFailureThrows()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: Unable to download API page: getaddrinfo failed");

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => sut.ListVideoFeedsAsync(Channel("https://www.youtube.com/@x"), CancellationToken.None));
    }

    [TestMethod]
    public async Task ResolvesChannelFromPlaylistJson()
    {
        const string JSON = """{"id":"@matthew_berman","channel":"Matthew Berman","channel_id":"UCawZsQWqfGSbCI5yjkdVkTA","channel_url":"https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA","uploader_url":"https://www.youtube.com/@matthew_berman"}""";
        runner.Behavior = _ => new ProcessResult(0, JSON, "");

        var details = await sut.ResolveChannelAsync("@matthew_berman", CancellationToken.None);

        Assert.AreEqual(new ChannelDetails("UCawZsQWqfGSbCI5yjkdVkTA", "https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA", "Matthew Berman"), details);
        Assert.AreEqual("https://www.youtube.com/@matthew_berman", runner.Invocations.Single().Arguments[^1]);
    }

    [TestMethod]
    public async Task NonYouTubeAddressReturnsNullWithoutRunningTheTool()
    {
        var details = await sut.ResolveChannelAsync("https://example.com/@someone", CancellationToken.None);

        Assert.IsNull(details);
        Assert.IsEmpty(runner.Invocations);
    }

    [TestMethod]
    public async Task MissingChannelReturnsNull()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: [youtube:tab] @nobody: Unable to download API page: HTTP Error 404: Not Found");

        var details = await sut.ResolveChannelAsync("@nobody", CancellationToken.None);

        Assert.IsNull(details);
    }

    [TestMethod]
    public async Task NetworkFailureWhileResolvingThrows()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: Unable to download webpage: getaddrinfo failed");

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => sut.ResolveChannelAsync("@nobody", CancellationToken.None));
    }

    //

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private YtDlpVideoCatalog sut = null!;

    private static string DetailsJson(string liveStatus) =>
        $$"""{"id":"zhnJo7s91ls","title":"Some title","upload_date":"20261031","timestamp":1793400000,"live_status":"{{liveStatus}}","channel":"C"}""";

    private static Channel Channel(string url) => new(1, url, "Test channel");
}