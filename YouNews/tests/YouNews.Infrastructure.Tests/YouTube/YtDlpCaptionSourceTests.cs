using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.YouTube;

namespace YouNews.Infrastructure.Tests.YouTube;

[TestClass]
public sealed class YtDlpCaptionSourceTests
{
    [TestInitialize]
    public void Setup()
    {
        environment = new TestEnvironment();
        runner = new FakeProcessRunner();
        sut = new YtDlpCaptionSource(new YtDlpRunner(runner, environment.Wrapped), new Json3Parser(), environment.Wrapped);
    }

    [TestCleanup]
    public void Cleanup() => environment.Dispose();

    [TestMethod]
    public async Task ReadsTranscriptAndDeletesEveryFile()
    {
        WriteFiles(("en", Caption("from en")));

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual("from en", transcript?.Segments.Single().Text);
        Assert.AreEqual(0, environment.WorkFiles().Length, string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task ParsesAutoCaptionsWithAppendEvents()
    {
        WriteFiles(("en", Json3ParserTests.AUTO_CAPTIONS));

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual(3, transcript?.Segments.Count);
        Assert.AreEqual("welcome to the channel", transcript?.Segments[1].Text);
    }

    [TestMethod]
    public async Task PrefersManualCaptionsOverAutomaticOnes()
    {
        WriteFiles(("en", Caption("auto en")), ("en-orig", Caption("auto original")), ("en-GB", Caption("manual british")));
        manualLanguages = ["en-GB"];

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual("manual british", transcript?.Segments.Single().Text);
    }

    [TestMethod]
    public async Task ManualPlainEnglishBeatsOriginalAutomatic()
    {
        WriteFiles(("en", Caption("manual en")), ("en-orig", Caption("auto original")));
        manualLanguages = ["en"];

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual("manual en", transcript?.Segments.Single().Text);
    }

    [TestMethod]
    public async Task WithoutManualCaptionsPrefersOriginalAutomaticOverTranslated()
    {
        WriteFiles(("en", Caption("translated")), ("en-orig", Caption("auto original")));

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.AreEqual("auto original", transcript?.Segments.Single().Text);
    }

    [TestMethod]
    public async Task VideoWithoutCaptionsGivesNullAndLeavesNothingBehind()
    {
        runner.Behavior = _ => new ProcessResult(0, "", "");

        var transcript = await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        Assert.IsNull(transcript);
        Assert.AreEqual(0, environment.WorkFiles().Length, string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task ToolFailureWithoutFilesThrowsAndStillCleansUp()
    {
        runner.Behavior = _ => new ProcessResult(1, "", "ERROR: [youtube] zhnJo7s91ls: Unable to download webpage");

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None));

        Assert.AreEqual(0, environment.WorkFiles().Length, string.Join(", ", environment.WorkFiles()));
    }

    [TestMethod]
    public async Task AsksForEnglishJson3SubtitlesInAFolderBelowTheWorkDirectory()
    {
        WriteFiles(("en", Caption("x")));

        await sut.GetCaptionsAsync("zhnJo7s91ls", CancellationToken.None);

        var call = runner.Invocations.Single();
        Assert.AreEqual("en.*,en", call.ValueAfter("--sub-langs"), call.ToString());
        Assert.AreEqual("json3", call.ValueAfter("--sub-format"));
        CollectionAssert.IsSubsetOf(new[] { "--skip-download", "--write-subs", "--write-auto-subs", "--no-update", "--no-warnings" }, call.Arguments.ToList());
        StringAssert.StartsWith(call.ValueAfter("-o"), environment.Options.WorkDirectory);
        Assert.AreEqual("https://www.youtube.com/watch?v=zhnJo7s91ls", call.Arguments[^1]);
    }

    //

    private TestEnvironment environment = null!;
    private FakeProcessRunner runner = null!;
    private YtDlpCaptionSource sut = null!;
    private string[] manualLanguages = [];

    private static string Caption(string text) => $$"""{"events":[{"tStartMs":1000,"segs":[{"utf8":"{{text}}"}]}]}""";

    /// <summary>Makes the fake tool write the given caption files, and an info file listing the manual languages.</summary>
    private void WriteFiles(params (string Language, string Json)[] files) =>
        runner.Behavior = call =>
        {
            var template = call.ValueAfter("-o");
            foreach (var (language, json) in files)
                File.WriteAllText(template.Replace("%(ext)s", $"{language}.json3"), json);

            var subtitles = string.Join(",", manualLanguages.Select(language => $"\"{language}\":[]"));
            File.WriteAllText(template.Replace("%(ext)s", "info.json"), "{\"subtitles\":{" + subtitles + "}}");
            return new ProcessResult(0, "", "");
        };
}