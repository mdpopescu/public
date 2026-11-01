using YouNews.Core.Model;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

/// <summary>Runs the real yt-dlp, ffmpeg and node found on PATH against YouTube.</summary>
[TestClass]
[TestCategory("Live")]
public sealed class YouTubeLiveTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        var processRunner = new ProcessRunner();
        ytDlp = new YtDlpRunner(processRunner, environment.Wrapped);
        catalog = new YtDlpVideoCatalog(ytDlp, new VideoErrorClassifier(), new ChannelAddressParser());
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task ResolvesMatthewBerman()
    {
        var details = await catalog.ResolveChannelAsync("@matthew_berman", CancellationToken.None);

        Assert.IsNotNull(details);
        Assert.AreEqual("Matthew Berman", details.Name);
        Assert.AreEqual("UCawZsQWqfGSbCI5yjkdVkTA", details.YouTubeId);
    }

    [TestMethod]
    public async Task UnknownHandleReturnsNull()
    {
        var details = await catalog.ResolveChannelAsync("@this-handle-does-not-exist-zzz12345", CancellationToken.None);

        Assert.IsNull(details, "an invalid handle must resolve to null");
    }

    [TestMethod]
    public async Task ListsVideoFeeds()
    {
        var channel = new Channel(1, "https://www.youtube.com/channel/UCawZsQWqfGSbCI5yjkdVkTA", "Matthew Berman");

        var feeds = await catalog.ListVideoFeedsAsync(channel, CancellationToken.None);

        Assert.HasCount(2, feeds);
        Assert.IsGreaterThan(10, feeds[0].Count, $"videos feed had {feeds[0].Count} items");
        Assert.IsTrue(feeds[0].All(id => id.Length == 11), string.Join(",", feeds[0].Take(5)));
    }

    [TestMethod]
    public async Task GetsDetailsOfAnOrdinaryVideo()
    {
        var details = await catalog.GetDetailsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(VideoAvailability.Eligible, details.Availability, details.Reason);
        Assert.AreNotEqual("zhnJo7s91ls", details.Title);
        Assert.IsNotNull(details.PublishedOn);
    }

    [TestMethod]
    public async Task MembersOnlyVideoIsPermanentlyUnavailable()
    {
        var details = await catalog.GetDetailsAsync("ZerTreaAgPA", CancellationToken.None);

        Assert.AreEqual(VideoAvailability.PermanentlyUnavailable, details.Availability, details.Reason);
        Assert.AreEqual("ZerTreaAgPA", details.Title);
    }

    [TestMethod]
    public async Task GetsCaptions()
    {
        var sut = new YtDlpCaptionSource(ytDlp, new Json3Parser(), environment.Wrapped);

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.IsNotNull(transcript);
        Assert.IsGreaterThan(100, transcript.WordCount, $"only {transcript.WordCount} words");
        Assert.IsEmpty(environment.WorkFiles(), string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task ToolHealthIsClean()
    {
        var sut = new ToolHealth(new ProcessRunner(), environment.Wrapped);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        Assert.IsEmpty(problems, string.Join(" | ", problems));
    }

    [TestMethod]
    public async Task CapturesThumbnailAndOneFrame()
    {
        var frames = new FrameExtractor(ytDlp, new ProcessRunner(), environment.Wrapped);
        var sut = new YouTubeImageSource(new ThumbnailDownloader(new DirectHttpClientFactory(), environment.Wrapped), frames);

        var images = await sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(30)], CancellationToken.None);

        Assert.HasCount(2, images, string.Join(", ", images.Select(image => image.RelativePath)));
        foreach (var image in images)
            Assert.IsGreaterThan(1000, new FileInfo(Path.Combine(environment.Options.ImagesDirectory, image.RelativePath)).Length, image.RelativePath);
        Assert.IsEmpty(environment.WorkFiles(), string.Join(", ", environment.WorkFiles()));
    }

    //

    private TestEnvironment environment = null!;
    private YtDlpRunner ytDlp = null!;
    private YtDlpVideoCatalog catalog = null!;

    private sealed class DirectHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new();
    }
}