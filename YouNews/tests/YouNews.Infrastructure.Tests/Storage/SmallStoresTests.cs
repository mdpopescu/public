using YouNews.Core.Model;
using YouNews.Infrastructure.Storage;

namespace YouNews.Infrastructure.Tests.Storage;

[TestClass]
public class SmallStoresTests
{
    [TestInitialize]
    public async Task Initialize() => await fixture.InitializeAsync();

    [TestCleanup]
    public void Cleanup() => fixture.Dispose();

    [TestMethod]
    public async Task InterestSaveAsync_NewList_ReplacesAllKeepingOrder()
    {
        var sut = new SqliteInterestStore(fixture.Factory);

        await sut.SaveAsync(["Zebras", "Apples", "Mangoes"], CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "Zebras", "Apples", "Mangoes" }, (await sut.GetAsync(CancellationToken.None)).ToArray());
    }

    [TestMethod]
    public async Task InterestSaveAsync_EmptyList_ClearsInterests()
    {
        var sut = new SqliteInterestStore(fixture.Factory);

        await sut.SaveAsync([], CancellationToken.None);

        Assert.IsEmpty((await sut.GetAsync(CancellationToken.None)));
    }

    [TestMethod]
    public async Task SetupGetLatestAsync_NeverSaved_ReturnsNull()
    {
        var sut = new SqliteSetupStore(fixture.Factory);

        Assert.IsNull(await sut.GetLatestAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task SetupSaveAsync_Snapshot_RoundTripsIncludingOffsetAndWarning()
    {
        var sut = new SqliteSetupStore(fixture.Factory);
        var snapshot = new SetupSnapshot(["gemma4-26b", "qwen3, \"quoted\""], "2x RTX 3090, 128 GB RAM", new DateTimeOffset(2026, 11, 1, 6, 0, 3, TimeSpan.FromHours(2)), "SSH timed out");

        await sut.SaveAsync(snapshot, CancellationToken.None);

        var loaded = await sut.GetLatestAsync(CancellationToken.None);
        Assert.IsNotNull(loaded);
        CollectionAssert.AreEqual(snapshot.Models.ToArray(), loaded.Models.ToArray());
        Assert.AreEqual(snapshot.Hardware, loaded.Hardware);
        Assert.AreEqual(snapshot.CheckedAt, loaded.CheckedAt);
        Assert.AreEqual(snapshot.CheckedAt.Offset, loaded.CheckedAt.Offset);
        Assert.AreEqual("SSH timed out", loaded.Warning);
    }

    [TestMethod]
    public async Task SetupSaveAsync_SecondSnapshot_ReplacesFirst()
    {
        var sut = new SqliteSetupStore(fixture.Factory);
        var when = new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero);
        await sut.SaveAsync(new SetupSnapshot(["old"], "old hw", when, "warn"), CancellationToken.None);

        await sut.SaveAsync(new SetupSnapshot(["new"], "new hw", when.AddDays(1), null), CancellationToken.None);

        var loaded = await sut.GetLatestAsync(CancellationToken.None);
        Assert.AreEqual("new hw", loaded!.Hardware);
        Assert.IsNull(loaded.Warning);
        CollectionAssert.AreEqual(new[] { "new" }, loaded.Models.ToArray());
    }

    [TestMethod]
    public async Task RefreshGetLatestAsync_NoRuns_ReturnsNull()
    {
        var sut = new SqliteRefreshLog(fixture.Factory);

        Assert.IsNull(await sut.GetLatestAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task RefreshRecordAsync_SeveralRuns_LatestIsTheLastRecorded()
    {
        var sut = new SqliteRefreshLog(fixture.Factory);
        var start = new DateTimeOffset(2026, 10, 31, 6, 0, 0, TimeSpan.FromHours(2));
        await sut.RecordAsync(new RefreshRun(start, start.AddMinutes(5), 3, 1, null), CancellationToken.None);
        var last = new RefreshRun(start.AddDays(1), start.AddDays(1).AddMinutes(9), 0, 4, "yt-dlp too old");
        await sut.RecordAsync(last, CancellationToken.None);

        var latest = await sut.GetLatestAsync(CancellationToken.None);

        Assert.AreEqual(last, latest);
        Assert.AreEqual(last.StartedAt.Offset, latest!.StartedAt.Offset);
    }

    //

    private readonly StorageFixture fixture = new();
}