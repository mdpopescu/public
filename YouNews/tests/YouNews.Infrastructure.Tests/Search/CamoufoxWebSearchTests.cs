using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Processes;
using YouNews.Infrastructure.Search;

namespace YouNews.Infrastructure.Tests.Search;

[TestClass]
public sealed class CamoufoxWebSearchTests
{
    [TestMethod]
    public async Task ParsesScriptOutputAndPassesQueryAsArgument()
    {
        var runner = new FakeProcessRunner(new ProcessResult(0, """
            [{"title":"Llama 5 released","url":"https://example.com/llama5","snippet":"A new model."},
             {"title":"Second","url":"https://example.org/two","snippet":""}]
            """, ""));
        var sut = Create(runner);

        var results = await sut.SearchAsync("llama 5 release", CancellationToken.None);

        Assert.HasCount(2, results);
        Assert.AreEqual("Llama 5 released", results[0].Title);
        Assert.AreEqual("https://example.com/llama5", results[0].Url);
        Assert.AreEqual("A new model.", results[0].Snippet);
        Assert.AreEqual("python-test", runner.FileName);
        Assert.AreEqual("llama 5 release", runner.Arguments[1], "the query should be the second argument");
        Assert.AreEqual(TimeSpan.FromSeconds(42), runner.Timeout);
        Assert.IsTrue(File.Exists(runner.Arguments[0]), $"script should be written to {runner.Arguments[0]}");
        StringAssert.Contains(await File.ReadAllTextAsync(runner.Arguments[0]), "AsyncCamoufox");
    }

    [TestMethod]
    public async Task CapsResultsAtMaximum()
    {
        var items = string.Join(",", Enumerable.Range(1, 8).Select(i => $$"""{"title":"T{{i}}","url":"https://e.com/{{i}}","snippet":"s"}"""));
        var sut = Create(new FakeProcessRunner(new ProcessResult(0, $"[{items}]", "")), maximumResults: 3);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.HasCount(3, results);
    }

    [TestMethod]
    public async Task ReturnsEmptyOnNonZeroExit()
    {
        var sut = Create(new FakeProcessRunner(new ProcessResult(1, "", "ModuleNotFoundError: camoufox")));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyOnGarbageOutput()
    {
        var sut = Create(new FakeProcessRunner(new ProcessResult(0, "Traceback... definitely not json", "")));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyOnTimeout()
    {
        var sut = Create(new FakeProcessRunner(new TimeoutException("too slow")));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ThrowsWhenCallerCancels()
    {
        var sut = Create(new FakeProcessRunner(new OperationCanceledException()));

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.SearchAsync("q", CancellationToken.None));
    }

    [TestMethod]
    public async Task BlankQueryRunsNothing()
    {
        var runner = new FakeProcessRunner(new ProcessResult(0, "[]", ""));
        var sut = Create(runner);

        var results = await sut.SearchAsync("", CancellationToken.None);

        Assert.IsEmpty(results);
        Assert.AreEqual(0, runner.Calls, "no process should be started");
    }

    [TestMethod]
    [TestCategory("Live")]
    public async Task LiveCamoufoxReturnsResults()
    {
        var options = Options.Create(new YouNewsOptions
        {
            DataDirectory = Path.Combine(AppContext.BaseDirectory, "search-live"),
            Search = { PythonPath = "python", TimeoutSeconds = 120, MaximumResults = 10 },
        });
        var sut = new CamoufoxWebSearch(new ProcessRunner(), options, NullLogger<CamoufoxWebSearch>.Instance);

        var results = await sut.SearchAsync("local LLM inference llama.cpp", CancellationToken.None);

        Console.WriteLine($"Camoufox returned {results.Count} results");
        foreach (var result in results)
            Console.WriteLine($"{result.Title} | {result.Url} | {result.Snippet}");
        Assert.IsNotEmpty(results);
        Assert.IsFalse(results.Any(it => it.Url.Contains("duckduckgo.com/l/")), "redirect links should be unwrapped");
    }

    //

    private static CamoufoxWebSearch Create(FakeProcessRunner runner, int maximumResults = 5)
    {
        var options = Options.Create(new YouNewsOptions
        {
            DataDirectory = Path.Combine(AppContext.BaseDirectory, "search-tests", Guid.NewGuid().ToString("N")),
            Search = { PythonPath = "python-test", TimeoutSeconds = 42, MaximumResults = maximumResults },
        });
        return new CamoufoxWebSearch(runner, options, NullLogger<CamoufoxWebSearch>.Instance);
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public int Calls { get; private set; }

        public string FileName { get; private set; } = "";

        public IReadOnlyList<string> Arguments { get; private set; } = [];

        public TimeSpan Timeout { get; private set; }

        public FakeProcessRunner(ProcessResult result) => this.result = result;

        public FakeProcessRunner(Exception failure) => this.failure = failure;

        public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string? workingDirectory, TimeSpan timeout, CancellationToken cancellationToken)
        {
            Calls++;
            FileName = fileName;
            Arguments = arguments;
            Timeout = timeout;
            return failure is null ? Task.FromResult(result!) : Task.FromException<ProcessResult>(failure);
        }

        //

        private readonly ProcessResult? result;
        private readonly Exception? failure;
    }
}