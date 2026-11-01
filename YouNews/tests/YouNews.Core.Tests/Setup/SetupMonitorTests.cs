using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Setup;

namespace YouNews.Core.Tests.Setup;

[TestClass]
public sealed class SetupMonitorTests
{
    [TestInitialize]
    public void Initialize()
    {
        time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.Zero));
        time.SetLocalTimeZone(TimeZoneInfo.Utc);
        inspector = new FakeInspector();
        store = new FakeStore();
        sut = new SetupMonitor(inspector, store, time);
    }

    [TestMethod]
    public async Task ReturnsStoredSnapshotCheckedTodayWithoutInspecting()
    {
        store.Latest = Snapshot(time.GetUtcNow().AddHours(-3));

        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.AreSame(store.Latest, result);
        Assert.AreEqual(0, inspector.Calls, "A snapshot from earlier today must not trigger a new inspection.");
    }

    [TestMethod]
    public async Task InspectsAndSavesWhenNothingIsStored()
    {
        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(inspector.Models, result.Models.ToList());
        Assert.AreEqual(inspector.Hardware, result.Hardware);
        Assert.AreEqual(time.GetUtcNow(), result.CheckedAt);
        Assert.IsNull(result.Warning);
        Assert.AreSame(result, store.Latest, "The new snapshot must be saved.");
    }

    [TestMethod]
    public async Task InspectsAgainWhenStoredSnapshotIsFromYesterday()
    {
        store.Latest = Snapshot(time.GetUtcNow().AddDays(-1));

        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.AreEqual(1, inspector.Calls);
        Assert.AreEqual(time.GetUtcNow(), result!.CheckedAt);
    }

    [TestMethod]
    public async Task InspectsAfterLocalMidnightEvenWhenLessThanADayHasPassed()
    {
        store.Latest = Snapshot(time.GetUtcNow().AddHours(13));
        time.Advance(TimeSpan.FromHours(13));
        await sut.GetCurrentAsync(CancellationToken.None);
        Assert.AreEqual(0, inspector.Calls, "Still the same day.");

        time.Advance(TimeSpan.FromHours(2));
        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.AreEqual(1, inspector.Calls, "Past midnight, the snapshot is from yesterday.");
        Assert.AreEqual(time.GetUtcNow(), result!.CheckedAt);
    }

    [TestMethod]
    public async Task KeepsOldDataWithWarningWhenInspectionFails()
    {
        var old = Snapshot(time.GetUtcNow().AddDays(-2));
        store.Latest = old;
        inspector.Failure = new HttpRequestException("connection refused");

        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual("connection refused", result.Warning);
        CollectionAssert.AreEqual(old.Models.ToList(), result.Models.ToList());
        Assert.AreEqual(old.Hardware, result.Hardware);
        Assert.AreEqual(old.CheckedAt, result.CheckedAt, "CheckedAt must stay at the last good check.");
        Assert.AreSame(result, store.Latest, "The warning must be saved.");
    }

    [TestMethod]
    public async Task ReturnsNullAndSavesNothingWhenInspectionFailsWithNoSnapshot()
    {
        inspector.Failure = new InvalidOperationException("no route to host");

        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.IsNull(result);
        Assert.IsNull(store.Latest);
    }

    [TestMethod]
    public async Task RetriesOnNextCallAndClearsWarningOnSuccess()
    {
        store.Latest = Snapshot(time.GetUtcNow().AddDays(-1));
        inspector.Failure = new HttpRequestException("down");
        await sut.GetCurrentAsync(CancellationToken.None);
        Assert.AreEqual(1, inspector.Calls);

        await sut.GetCurrentAsync(CancellationToken.None);
        Assert.AreEqual(2, inspector.Calls, "A snapshot with a warning must be retried even on the same day.");

        inspector.Failure = null;
        var result = await sut.GetCurrentAsync(CancellationToken.None);

        Assert.AreEqual(3, inspector.Calls);
        Assert.IsNull(result!.Warning);
        CollectionAssert.AreEqual(inspector.Models, result.Models.ToList());

        await sut.GetCurrentAsync(CancellationToken.None);
        Assert.AreEqual(3, inspector.Calls, "Once healthy again, no more inspections today.");
    }

    [TestMethod]
    public async Task PropagatesCancellationWithoutSavingWarning()
    {
        var old = Snapshot(time.GetUtcNow().AddDays(-1));
        store.Latest = old;
        using var source = new CancellationTokenSource();
        await source.CancelAsync();
        inspector.Failure = new OperationCanceledException(source.Token);

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.GetCurrentAsync(source.Token));

        Assert.AreSame(old, store.Latest, "A cancelled check is not a server failure.");
    }

    //

    private FakeTimeProvider time = null!;
    private FakeInspector inspector = null!;
    private FakeStore store = null!;
    private SetupMonitor sut = null!;

    private static SetupSnapshot Snapshot(DateTimeOffset checkedAt) =>
        new(["old-model-a", "old-model-b"], "GPUs: old. CPU: old. RAM: 1Gi total.", checkedAt, null);

    private sealed class FakeInspector : IServerInspector
    {
        public int Calls { get; private set; }

        public Exception? Failure { get; set; }

        public List<string> Models { get; } = ["gemma4-26b", "qwen3-8b"];

        public string Hardware => "GPUs: NVIDIA GeForce RTX 5060 Ti, 16376 MiB. CPU: AMD Ryzen 9 3900X 12-Core Processor. RAM: 62Gi total.";

        public Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken)
        {
            Calls++;
            if (Failure != null)
                throw Failure;

            return Task.FromResult<IReadOnlyList<string>>(Models);
        }

        public Task<string> DescribeHardwareAsync(CancellationToken cancellationToken) => Task.FromResult(Hardware);
    }

    private sealed class FakeStore : ISetupStore
    {
        public SetupSnapshot? Latest { get; set; }

        public Task<SetupSnapshot?> GetLatestAsync(CancellationToken cancellationToken) => Task.FromResult(Latest);

        public Task SaveAsync(SetupSnapshot snapshot, CancellationToken cancellationToken)
        {
            Latest = snapshot;
            return Task.CompletedTask;
        }
    }
}