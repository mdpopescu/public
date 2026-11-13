using Microsoft.Extensions.Logging;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class YtDlpRunnerTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        runner = new FakeProcessRunner();
        logger = new RecordingLogger<YtDlpRunner>();
        sut = new YtDlpRunner(runner, environment.Wrapped, logger);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task LeavesWarningsOn()
    {
        await sut.RunAsync(["https://www.youtube.com/watch?v=zhnJo7s91ls"], CancellationToken.None);

        var call = runner.Invocations.Single();
        CollectionAssert.DoesNotContain(call.Arguments.ToList(), "--no-warnings", call.ToString());
    }

    [TestMethod]
    public async Task LogsEachWarningWithWhatItWasAbout()
    {
        runner.Behavior = _ => new ProcessResult(
            1,
            "",
            "[youtube] zhnJo7s91ls: Downloading webpage\r\nWARNING: no impersonate target is available\r\nERROR: HTTP Error 429: Too Many Requests\r\nWARNING: second\r\n");

        await sut.RunAsync(["https://www.youtube.com/watch?v=zhnJo7s91ls"], CancellationToken.None);

        var logged = string.Join(" | ", logger.Entries);
        Assert.HasCount(2, logger.Entries, logged);
        Assert.IsTrue(logger.Entries.All(entry => entry.Level == LogLevel.Warning), logged);
        StringAssert.Contains(logger.Entries[0].Message, "zhnJo7s91ls", logged);
        StringAssert.Contains(logger.Entries[0].Message, "WARNING: no impersonate target is available", logged);
        StringAssert.Contains(logger.Entries[1].Message, "WARNING: second", logged);
    }

    [TestMethod]
    public async Task LogsNothingWithoutWarnings()
    {
        runner.Behavior = _ => new ProcessResult(0, "", "[youtube] zhnJo7s91ls: Downloading webpage\n");

        await sut.RunAsync(["https://www.youtube.com/watch?v=zhnJo7s91ls"], CancellationToken.None);

        Assert.IsEmpty(logger.Entries, string.Join(" | ", logger.Entries));
    }

    //

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private RecordingLogger<YtDlpRunner> logger = null!;
    private YtDlpRunner sut = null!;
}