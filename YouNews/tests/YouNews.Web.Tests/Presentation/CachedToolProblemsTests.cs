using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Ports;
using YouNews.Web.Presentation;

namespace YouNews.Web.Tests.Presentation;

[TestClass]
public sealed class CachedToolProblemsTests
{
    [TestInitialize]
    public void Initialize()
    {
        health = new CountingToolHealth();
        time = new FakeTimeProvider();
        sut = new CachedToolProblems(health, time);
    }

    [TestMethod]
    public async Task GetAsync_WithinTenMinutes_ChecksOnlyOnce()
    {
        health.Problems = ["ffmpeg missing"];

        await sut.GetAsync(CancellationToken.None);
        time.Advance(TimeSpan.FromMinutes(9));
        var second = await sut.GetAsync(CancellationToken.None);

        Assert.AreEqual(1, health.Calls, "the check starts processes, so it must be cached");
        CollectionAssert.AreEqual(new[] { "ffmpeg missing" }, second.ToArray());
    }

    [TestMethod]
    public async Task GetAsync_AfterTenMinutes_ChecksAgain()
    {
        await sut.GetAsync(CancellationToken.None);
        time.Advance(TimeSpan.FromMinutes(10));
        health.Problems = ["now broken"];

        var result = await sut.GetAsync(CancellationToken.None);

        Assert.AreEqual(2, health.Calls);
        CollectionAssert.AreEqual(new[] { "now broken" }, result.ToArray());
    }

    [TestMethod]
    public async Task GetAsync_CheckFails_ReportsAProblemInsteadOfThrowing()
    {
        health.Failure = new InvalidOperationException("cannot start yt-dlp");

        var result = await sut.GetAsync(CancellationToken.None);

        Assert.HasCount(1, result);
        StringAssert.Contains(result[0], "cannot start yt-dlp");
    }

    [TestMethod]
    public async Task GetAsync_AfterAFailure_TriesAgainSooner()
    {
        health.Failure = new InvalidOperationException("boom");
        await sut.GetAsync(CancellationToken.None);
        health.Failure = null;
        time.Advance(TimeSpan.FromMinutes(1));

        var result = await sut.GetAsync(CancellationToken.None);

        Assert.IsEmpty(result, "the tools recovered, so the banner should go away within a minute");
    }

    //

    private CountingToolHealth health = null!;
    private FakeTimeProvider time = null!;
    private CachedToolProblems sut = null!;

    private sealed class CountingToolHealth : IToolHealth
    {
        public int Calls { get; private set; }

        public IReadOnlyList<string> Problems { get; set; } = [];

        public Exception? Failure { get; set; }

        public Task<IReadOnlyList<string>> FindProblemsAsync(CancellationToken cancellationToken)
        {
            Calls++;
            return Failure is null ? Task.FromResult(Problems) : Task.FromException<IReadOnlyList<string>>(Failure);
        }
    }
}