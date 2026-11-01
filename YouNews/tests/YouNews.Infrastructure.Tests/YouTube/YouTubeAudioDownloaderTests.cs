using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class YouTubeAudioDownloaderTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        runner = new FakeProcessRunner();
        sut = new YouTubeAudioDownloader(new YtDlpRunner(runner, environment.Wrapped), environment.Wrapped);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task ReturnsTheDownloadedFileInTheWorkDirectory()
    {
        runner.Behavior = invocation => WriteFiles(invocation, "m4a");

        var path = await sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.IsTrue(File.Exists(path), path);
        Assert.AreEqual(environment.Options.WorkDirectory, Path.GetDirectoryName(path));
        Assert.StartsWith("zhnJo7s91ls-", Path.GetFileName(path));
        Assert.EndsWith(".m4a", path);
    }

    [TestMethod]
    public async Task AsksForALowBitrateAudioTrackOfTheWatchPage()
    {
        runner.Behavior = invocation => WriteFiles(invocation, "webm");

        await sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None);

        var invocation = runner.Invocations.Single();
        Assert.AreEqual("ba[abr<=64]/ba", invocation.ValueAfter("-f"), invocation.ToString());
        Assert.AreEqual("https://www.youtube.com/watch?v=zhnJo7s91ls", invocation.Arguments[^1], invocation.ToString());
    }

    [TestMethod]
    public async Task IgnoresAPartialFileLeftByYtDlp()
    {
        runner.Behavior = invocation =>
        {
            WriteFiles(invocation, "m4a.part");
            return WriteFiles(invocation, "m4a");
        };

        var path = await sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.EndsWith(".m4a", path);
    }

    [TestMethod]
    public async Task TimeoutDeletesPartialFiles()
    {
        runner.Behavior = invocation =>
        {
            WriteFiles(invocation, "m4a.part");
            throw new TimeoutException("yt-dlp took too long");
        };

        await Assert.ThrowsExactlyAsync<TimeoutException>(() => sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.IsEmpty(Directory.GetFiles(environment.Options.WorkDirectory), "A timed-out download must not leave files behind.");
    }

    [TestMethod]
    public async Task FailureReportsTheToolMessageAndDeletesPartialFiles()
    {
        runner.Behavior = invocation =>
        {
            WriteFiles(invocation, "m4a.part");
            return new ProcessResult(1, "", "ERROR: Sign in to confirm you are not a bot\n");
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.Contains("zhnJo7s91ls", exception.Message);
        Assert.Contains("Sign in to confirm you are not a bot", exception.Message);
        Assert.AreEqual(0, environment.WorkFiles().Length, string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task SuccessWithoutAnyFileIsAFailure()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DownloadAudioAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.Contains("could not download the audio", exception.Message);
    }

    [TestMethod]
    public async Task RejectsAnIdentifierThatIsNotSafeInAPath()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => sut.DownloadAudioAsync("../evil", CancellationToken.None));

        Assert.AreEqual(0, runner.Invocations.Count, "no tool may run for a bad identifier");
    }

    //

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private YouTubeAudioDownloader sut = null!;

    private ProcessResult WriteFiles(FakeProcessRunner.Invocation invocation, string extension)
    {
        Directory.CreateDirectory(environment.Options.WorkDirectory);
        File.WriteAllText(invocation.ValueAfter("-o").Replace("%(ext)s", extension), "audio");
        return new ProcessResult(0, "", "");
    }
}