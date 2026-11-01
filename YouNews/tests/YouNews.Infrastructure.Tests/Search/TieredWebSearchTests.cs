using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Search;

namespace YouNews.Infrastructure.Tests.Search;

[TestClass]
public sealed class TieredWebSearchTests
{
    [TestMethod]
    public async Task EnoughPrimaryResultsMeansSecondaryIsNotCalled()
    {
        var primary = new FakeSearch(Result("a"), Result("b"), Result("c"));
        var secondary = new FakeSearch(Result("d"));
        var sut = Create(primary, secondary);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "https://a.com/", "https://b.com/", "https://c.com/" }, results.Select(it => it.Url).ToArray());
        Assert.AreEqual(0, secondary.Calls, "secondary should not be called");
    }

    [TestMethod]
    public async Task ThinPrimaryResultsAreMergedWithSecondaryAndDeduplicated()
    {
        var primary = new FakeSearch(Result("a"), Result("b"));
        var secondary = new FakeSearch(
            new WebResult("A again", "http://a.com", ""),
            new WebResult("B again", "https://B.com", ""),
            Result("c"));
        var sut = Create(primary, secondary);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "https://a.com/", "https://b.com/", "https://c.com/" }, results.Select(it => it.Url).ToArray());
        Assert.AreEqual("A", results[0].Title, "primary entries should win over duplicates");
        Assert.AreEqual(1, secondary.Calls);
    }

    [TestMethod]
    public async Task CapsMergedResultsAtMaximum()
    {
        var primary = new FakeSearch(Result("a"));
        var secondary = new FakeSearch(Result("b"), Result("c"), Result("d"), Result("e"));
        var sut = Create(primary, secondary, maximumResults: 3);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "https://a.com/", "https://b.com/", "https://c.com/" }, results.Select(it => it.Url).ToArray());
    }

    [TestMethod]
    public async Task BothEmptyGivesEmpty()
    {
        var primary = new FakeSearch();
        var secondary = new FakeSearch();
        var sut = Create(primary, secondary);

        var results = await sut.SearchAsync("q", CancellationToken.None);

        Assert.IsEmpty(results);
        Assert.AreEqual(1, secondary.Calls, "an empty primary should fall back");
    }

    [TestMethod]
    public async Task BlankQueryCallsNothing()
    {
        var primary = new FakeSearch(Result("a"));
        var secondary = new FakeSearch(Result("b"));
        var sut = Create(primary, secondary);

        var results = await sut.SearchAsync("   ", CancellationToken.None);

        Assert.IsEmpty(results);
        Assert.AreEqual(0, primary.Calls, "primary should not be called");
        Assert.AreEqual(0, secondary.Calls, "secondary should not be called");
    }

    //

    private static WebResult Result(string name) => new(name.ToUpperInvariant(), $"https://{name}.com/", $"About {name}");

    private static TieredWebSearch Create(IWebSearch primary, IWebSearch secondary, int maximumResults = 5)
    {
        var options = Options.Create(new YouNewsOptions { Search = { MinimumResults = 3, MaximumResults = maximumResults } });
        return new TieredWebSearch(primary, secondary, options);
    }

    private sealed class FakeSearch(params WebResult[] results) : IWebSearch
    {
        public int Calls { get; private set; }

        public Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<WebResult>>(results);
        }
    }
}