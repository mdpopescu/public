using System.ComponentModel;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class ToolHealthTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        environment.Options.Tools.MinimumYtDlpVersion = "2026.10.26";
        runner = new FakeProcessRunner();
        sut = new ToolHealth(runner, environment.Wrapped);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    [DataRow("2026.10.26")]
    [DataRow("2026.11.03\n")]
    [DataRow("2027.01.01")]
    [DataRow("2026.10.26.232815")]
    public async Task RecentEnoughYtDlpHasNoProblems(string version)
    {
        Tools(ytDlp: version);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        Assert.IsEmpty(problems, string.Join(" | ", problems));
    }

    [TestMethod]
    [DataRow("2026.07.04")]
    [DataRow("2025.12.31")]
    [DataRow("2026.10.25")]
    public async Task OlderYtDlpGivesProblemWithTheFix(string version)
    {
        Tools(ytDlp: version);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        var problem = problems.Single();
        StringAssert.Contains(problem, version);
        StringAssert.Contains(problem, "2026.10.26");
        StringAssert.Contains(problem, "pip install --upgrade \"yt-dlp[default]\"");
    }

    [TestMethod]
    public async Task MissingYtDlpGivesProblemWithTheFix()
    {
        Tools(ytDlp: null);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        var problem = problems.Single();
        StringAssert.Contains(problem, "yt-dlp could not be started");
        StringAssert.Contains(problem, "pip install --upgrade \"yt-dlp[default]\"");
    }

    [TestMethod]
    public async Task MissingFfmpegAndNodeEachGiveOneProblem()
    {
        Tools(ytDlp: "2026.10.26", ffmpeg: false, node: false);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        Assert.HasCount(2, problems, string.Join(" | ", problems));
        StringAssert.Contains(problems[0], "ffmpeg");
        StringAssert.Contains(problems[1], "node");
    }

    [TestMethod]
    public async Task ChecksTheConfiguredPathsAndVersions()
    {
        environment.Options.Tools.YtDlpPath = @"C:\tools\yt-dlp.exe";
        Tools(ytDlp: "2026.10.26");

        await sut.FindProblemsAsync(CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { @"C:\tools\yt-dlp.exe --version", @"C:\tools\yt-dlp.exe --no-update --list-impersonate-targets", "ffmpeg -version", "node --version" },
            runner.Invocations.Select(call => call.ToString()).ToArray());
    }

    [TestMethod]
    public async Task YtDlpWithoutAnyImpersonationTargetGivesProblemWithTheFix()
    {
        Tools(ytDlp: "2026.10.26", impersonateTargets: UNAVAILABLE_TARGETS);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        var problem = problems.Single();
        StringAssert.Contains(problem, "impersonate");
        StringAssert.Contains(problem, "pip install --upgrade \"yt-dlp[default,curl-cffi]\"");
    }

    [TestMethod]
    public async Task FailedImpersonationListingGivesProblem()
    {
        Tools(ytDlp: "2026.10.26");
        var tools = runner.Behavior;
        runner.Behavior = call => call.Arguments.Contains("--list-impersonate-targets") ? new ProcessResult(2, "", "error: no such option") : tools(call);

        var problems = await sut.FindProblemsAsync(CancellationToken.None);

        StringAssert.Contains(problems.Single(), "impersonate");
    }

    //

    private const string AVAILABLE_TARGETS = """
        [info] Available impersonate targets
        Client          OS           Source
        --------------------------------------
        Chrome-133      Macos-15     curl_cffi
        Chrome-136      Macos-15     curl_cffi
        """;

    private const string UNAVAILABLE_TARGETS = """
        [info] Available impersonate targets
        Client    OS   Source
        --------------------------------------------
        Chrome    -    curl_cffi (unavailable)
        Firefox   -    curl_cffi>=0.10 (unavailable)
        """;

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private ToolHealth sut = null!;

    private static ProcessResult YtDlp(FakeProcessRunner.Invocation call, string? version, string impersonateTargets)
    {
        if (version == null)
            throw new Win32Exception(2);

        return call.Arguments.Contains("--list-impersonate-targets") ? new ProcessResult(0, impersonateTargets, "") : new ProcessResult(0, version, "");
    }

    private void Tools(string? ytDlp, bool ffmpeg = true, bool node = true, string impersonateTargets = AVAILABLE_TARGETS) =>
        runner.Behavior = call => call.FileName switch
        {
            var name when name.EndsWith("yt-dlp.exe", StringComparison.Ordinal) || name == "yt-dlp" => YtDlp(call, ytDlp, impersonateTargets),
            "ffmpeg" => ffmpeg ? new ProcessResult(0, "ffmpeg version 7", "") : throw new Win32Exception(2),
            _ => node ? new ProcessResult(0, "v22.0.0", "") : throw new Win32Exception(2),
        };
}