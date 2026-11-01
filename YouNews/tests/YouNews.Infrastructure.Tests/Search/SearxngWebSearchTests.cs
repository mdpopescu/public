using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Search;

namespace YouNews.Infrastructure.Tests.Search;

[TestClass]
public sealed class SearxngWebSearchTests
{
    [TestMethod]
    public async Task MapsResultsAndBuildsJsonQuery()
    {
        var handler = new StubHandler(_ => Json("""
            {"query":"ternary bonsai","results":[
              {"title":"Bonsai models","url":"https://example.com/bonsai","content":"Ternary weights."},
              {"title":"Paper","url":"https://arxiv.org/abs/1","content":"Research."}]}
            """));
        var sut = Create(handler);

        var results = await sut.SearchAsync("ternary bonsai", CancellationToken.None);

        Assert.HasCount(2, results, "both results should be mapped");
        Assert.AreEqual("Bonsai models", results[0].Title);
        Assert.AreEqual("https://example.com/bonsai", results[0].Url);
        Assert.AreEqual("Ternary weights.", results[0].Snippet);
        Assert.AreEqual("http://searx.test:8888/search?q=ternary%20bonsai&format=json", handler.Requests.Single().AbsoluteUri);
    }

    [TestMethod]
    public async Task CapsResultsAtMaximum()
    {
        var items = string.Join(",", Enumerable.Range(1, 9).Select(i => $$"""{"title":"T{{i}}","url":"https://e.com/{{i}}","content":"c"}"""));
        var sut = Create(new StubHandler(_ => Json($$"""{"results":[{{items}}]}""")), maximumResults: 4);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.HasCount(4, results);
        Assert.AreEqual("https://e.com/1", results[0].Url, "the first results should be kept");
    }

    [TestMethod]
    public async Task ReturnsEmptyWhenNothingFound()
    {
        var sut = Create(new StubHandler(_ => Json("""{"results":[]}""")));

        var results = await sut.SearchAsync("nothing", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyWhenRateLimited()
    {
        var sut = Create(new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyWhenJsonIsMalformed()
    {
        var sut = Create(new StubHandler(_ => Json("<html>not json</html>")));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyWhenServerIsUnreachable()
    {
        var sut = Create(new StubHandler(_ => throw new HttpRequestException("connection refused")));

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ReturnsEmptyWhenRequestTimesOut()
    {
        var sut = Create(new StubHandler(async (_, token) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30), token);
            return Json("""{"results":[]}""");
        }), timeoutSeconds: 1);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
    }

    [TestMethod]
    public async Task ThrowsWhenCallerCancels()
    {
        using var source = new CancellationTokenSource();
        var sut = Create(new StubHandler(async (_, token) =>
        {
            await source.CancelAsync();
            await Task.Delay(TimeSpan.FromSeconds(30), token);
            return Json("{}");
        }));

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.SearchAsync("q", source.Token));
    }

    [TestMethod]
    public async Task BlankQueryMakesNoRequest()
    {
        var handler = new StubHandler(_ => Json("{}"));
        var sut = Create(handler);

        var results = await sut.SearchAsync("  ", CancellationToken.None);

        Assert.IsEmpty(results);
        Assert.IsEmpty(handler.Requests, "no request should be sent");
    }

    [TestMethod]
    [TestCategory("Live")]
    public async Task LiveSearxngReturnsResults()
    {
        var options = Options.Create(new YouNewsOptions());
        using var client = new HttpClient();
        var sut = new SearxngWebSearch(client, options, NullLogger<SearxngWebSearch>.Instance);

        var results = await sut.SearchAsync("local LLM inference", CancellationToken.None);

        Console.WriteLine($"SearXNG at {options.Value.Search.SearxngUrl} returned {results.Count} results");
        Assert.IsNotEmpty(results, $"SearXNG at {options.Value.Search.SearxngUrl} returned nothing");
    }

    //

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static SearxngWebSearch Create(StubHandler handler, int maximumResults = 5, int timeoutSeconds = 30)
    {
        var options = Options.Create(new YouNewsOptions
        {
            Search = { SearxngUrl = "http://searx.test:8888/", MaximumResults = maximumResults, TimeoutSeconds = timeoutSeconds },
        });
        return new SearxngWebSearch(new HttpClient(handler), options, NullLogger<SearxngWebSearch>.Instance);
    }

    private sealed class StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            : this((request, _) => Task.FromResult(respond(request)))
        {
        }

        //

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return respond(request, cancellationToken);
        }
    }
}