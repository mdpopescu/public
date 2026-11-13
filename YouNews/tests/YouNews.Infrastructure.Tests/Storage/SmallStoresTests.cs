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

    [TestMethod]
    public async Task AlertGetAsync_NothingRaised_ReturnsNull()
    {
        var sut = new SqliteRunAlertStore(fixture.Factory);

        Assert.IsNull(await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task AlertRaiseAsync_Alert_RoundTripsIncludingOffset()
    {
        var sut = new SqliteRunAlertStore(fixture.Factory);
        var raisedAt = new DateTimeOffset(2026, 11, 1, 23, 30, 0, TimeSpan.FromHours(2));

        await sut.RaiseAsync("YouTube refused 5 requests in a row.", raisedAt, CancellationToken.None);

        var loaded = await sut.GetAsync(CancellationToken.None);
        Assert.AreEqual(new RunAlert("YouTube refused 5 requests in a row.", raisedAt), loaded);
        Assert.AreEqual(raisedAt.Offset, loaded!.RaisedAt.Offset);
    }

    [TestMethod]
    public async Task AlertRaiseAsync_AlertAlreadyWaiting_KeepsOnlyTheNewOne()
    {
        var sut = new SqliteRunAlertStore(fixture.Factory);
        var when = new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero);
        await sut.RaiseAsync("first", when, CancellationToken.None);

        await sut.RaiseAsync("second", when.AddDays(1), CancellationToken.None);

        await using var connection = await fixture.Factory.OpenAsync(CancellationToken.None);
        Assert.AreEqual(1, await Dapper.SqlMapper.ExecuteScalarAsync<int>(connection, "SELECT COUNT(*) FROM run_alert"));
        Assert.AreEqual(new RunAlert("second", when.AddDays(1)), await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task AlertDismissAsync_AlertWaiting_RemovesIt()
    {
        var sut = new SqliteRunAlertStore(fixture.Factory);
        await sut.RaiseAsync("stopped", DateTimeOffset.UnixEpoch, CancellationToken.None);

        await sut.DismissAsync(CancellationToken.None);

        Assert.IsNull(await sut.GetAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task AlertDismissAsync_NothingWaiting_DoesNothing()
    {
        var sut = new SqliteRunAlertStore(fixture.Factory);

        await sut.DismissAsync(CancellationToken.None);

        Assert.IsNull(await sut.GetAsync(CancellationToken.None));
    }

    //

    private readonly StorageFixture fixture = new();
}