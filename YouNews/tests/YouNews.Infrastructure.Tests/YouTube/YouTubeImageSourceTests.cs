using System.Net;
using YouNews.Core.Model;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class YouTubeImageSourceTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        runner = new FakeProcessRunner();
        handler = new StubHandler();
        var thumbnails = new ThumbnailDownloader(new StubHttpClientFactory(handler), environment.Wrapped);
        var frames = new FrameExtractor(new YtDlpRunner(runner, environment.Wrapped), runner, environment.Wrapped);
        sut = new YouTubeImageSource(thumbnails, frames);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task StoresMaximumResolutionThumbnail()
    {
        var images = await sut.CaptureAsync("zhnJo7s91ls", [], CancellationToken.None);

        Assert.AreEqual(new EntryImage("zhnJo7s91ls/thumbnail.jpg", ImageKind.Thumbnail), images.Single());
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, File.ReadAllBytes(Path.Combine(environment.Options.ImagesDirectory, "zhnJo7s91ls", "thumbnail.jpg")));
        Assert.IsEmpty(runner.Invocations, "no frames requested, so no tool must run");
    }

    [TestMethod]
    public async Task FallsBackToHighQualityThumbnailWhenMaximumIs404()
    {
        handler.Respond = uri => uri.AbsoluteUri.EndsWith("maxresdefault.jpg", StringComparison.Ordinal) ? HttpStatusCode.NotFound : HttpStatusCode.OK;

        var images = await sut.CaptureAsync("zhnJo7s91ls", [], CancellationToken.None);

        Assert.HasCount(1, images);
        CollectionAssert.AreEqual(
            new[] { "https://i.ytimg.com/vi/zhnJo7s91ls/maxresdefault.jpg", "https://i.ytimg.com/vi/zhnJo7s91ls/hqdefault.jpg" },
            handler.Requested.Select(uri => uri.AbsoluteUri).ToArray());
    }

    [TestMethod]
    public async Task NoThumbnailAtAllGivesEmptyResultWithoutThrowing()
    {
        handler.Respond = _ => HttpStatusCode.NotFound;

        var images = await sut.CaptureAsync("zhnJo7s91ls", [], CancellationToken.None);

        Assert.IsEmpty(images);
    }

    [TestMethod]
    public async Task ServerErrorOnThumbnailGivesEmptyResultWithoutThrowing()
    {
        handler.Respond = _ => HttpStatusCode.InternalServerError;

        var images = await sut.CaptureAsync("zhnJo7s91ls", [], CancellationToken.None);

        Assert.IsEmpty(images);
    }

    [TestMethod]
    public async Task ExtractsFramesAndDeletesTheDownloadedVideo()
    {
        FakeTools(frameLimit: TimeSpan.FromMinutes(10));

        var images = await sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(75), TimeSpan.FromSeconds(300)], CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "zhnJo7s91ls/thumbnail.jpg", "zhnJo7s91ls/frame-75.jpg", "zhnJo7s91ls/frame-300.jpg" },
            images.Select(image => image.RelativePath).ToArray());
        Assert.AreEqual(ImageKind.Frame, images[1].Kind);
        Assert.IsTrue(File.Exists(Path.Combine(environment.Options.ImagesDirectory, "zhnJo7s91ls", "frame-75.jpg")));
        Assert.IsEmpty(environment.WorkFiles(), "video left behind: " + string.Join(", ", environment.WorkFiles()));
        var download = runner.Invocations.First(call => call.FileName == "yt-dlp");
        Assert.AreEqual("bv*[height<=480][ext=mp4]/b[height<=480]", download.ValueAfter("-f"), download.ToString());
    }

    [TestMethod]
    public async Task FrameAfterTheEndOfTheVideoIsLeftOut()
    {
        FakeTools(frameLimit: TimeSpan.FromMinutes(2));

        var images = await sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(60), TimeSpan.FromMinutes(30)], CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "zhnJo7s91ls/thumbnail.jpg", "zhnJo7s91ls/frame-60.jpg" }, images.Select(image => image.RelativePath).ToArray());
        Assert.IsFalse(File.Exists(Path.Combine(environment.Options.ImagesDirectory, "zhnJo7s91ls", "frame-1800.jpg")));
    }

    [TestMethod]
    public async Task FailedVideoDownloadLeavesOnlyTheThumbnail()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: unable to download video data: HTTP Error 403: Forbidden");

        var images = await sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(60)], CancellationToken.None);

        Assert.HasCount(1, images);
        Assert.AreEqual(ImageKind.Thumbnail, images[0].Kind);
        Assert.HasCount(1, runner.Invocations, "ffmpeg must not run without a video");
    }

    [TestMethod]
    public async Task VideoIsDeletedEvenWhenFfmpegFails()
    {
        FakeTools(frameLimit: TimeSpan.FromMinutes(10), ffmpegFails: true);

        var images = await sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(60)], CancellationToken.None);

        Assert.HasCount(1, images);
        Assert.IsEmpty(environment.WorkFiles(), "video left behind: " + string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task CancellationIsNotSwallowed()
    {
        runner.Behavior = _ => throw new OperationCanceledException();

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.CaptureAsync("zhnJo7s91ls", [TimeSpan.FromSeconds(60)], CancellationToken.None));
    }

    //

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private StubHandler handler = null!;
    private YouTubeImageSource sut = null!;

    /// <summary>yt-dlp writes a video file; ffmpeg writes the frame unless the time is past the end.</summary>
    private void FakeTools(TimeSpan frameLimit, bool ffmpegFails = false) =>
        runner.Behavior = call =>
        {
            if (call.FileName == "yt-dlp")
            {
                File.WriteAllBytes(call.ValueAfter("-o").Replace("%(ext)s", "mp4"), [9, 9, 9]);
                return new ProcessResult(0, "", "");
            }

            var seconds = double.Parse(call.ValueAfter("-ss"), System.Globalization.CultureInfo.InvariantCulture);
            if (!ffmpegFails && seconds < frameLimit.TotalSeconds)
                File.WriteAllBytes(call.Arguments[^1], [7, 7]);
            return new ProcessResult(0, "", "");
        };

    private sealed class StubHandler : HttpMessageHandler
    {
        public List<Uri> Requested { get; } = [];

        public Func<Uri, HttpStatusCode> Respond { get; set; } = _ => HttpStatusCode.OK;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requested.Add(request.RequestUri!);
            var status = Respond(request.RequestUri!);
            return Task.FromResult(new HttpResponseMessage(status) { Content = new ByteArrayContent([1, 2, 3]) });
        }
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, false);
    }
}