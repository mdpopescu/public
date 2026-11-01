using Microsoft.Extensions.Time.Testing;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Core.Refresh;
using YouNews.Web.Hosting;

namespace YouNews.Web.Tests;

[TestClass]
public sealed class DailyRefreshServiceTests
{
    [TestMethod]
    public async Task CatchesUpOnStartWhenTodaysRunWasMissed()
    {
        var trigger = new CountingTrigger();
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero));
        using var sut = new DailyRefreshService(trigger, new FakeLog(), new RefreshSchedule(new TimeOnly(6, 0), TimeZoneInfo.Utc), time);

        await sut.StartAsync(CancellationToken.None);
        await trigger.WaitForStartsAsync(1);
        await sut.StopAsync(CancellationToken.None);

        Assert.AreEqual(1, trigger.Starts);
    }

    [TestMethod]
    public async Task RunsAtTheScheduledTime()
    {
        var trigger = new CountingTrigger();
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 5, 0, 0, TimeSpan.Zero));
        using var sut = new DailyRefreshService(trigger, new FakeLog(), new RefreshSchedule(new TimeOnly(6, 0), TimeZoneInfo.Utc), time);

        await sut.StartAsync(CancellationToken.None);
        await Task.Delay(100);
        Assert.AreEqual(0, trigger.Starts, "It is not yet six o'clock.");

        time.Advance(TimeSpan.FromHours(1));
        await trigger.WaitForStartsAsync(1);
        await sut.StopAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task AFailingLogDoesNotStopTheService()
    {
        var trigger = new CountingTrigger();
        var log = new FakeLog { Failure = new InvalidOperationException("database is locked") };
        var time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero));
        using var sut = new DailyRefreshService(trigger, log, new RefreshSchedule(new TimeOnly(6, 0), TimeZoneInfo.Utc), time);

        await sut.StartAsync(CancellationToken.None);
        await Task.Delay(100);
        time.Advance(TimeSpan.FromDays(1));
        await trigger.WaitForStartsAsync(1);

        Assert.IsFalse(sut.ExecuteTask!.IsFaulted, "A fault here would shut down the whole Windows service.");
        await sut.StopAsync(CancellationToken.None);
    }

    //

    private sealed class CountingTrigger : IRefreshTrigger
    {
        public int Starts => Volatile.Read(ref starts);

        public bool TryStart()
        {
            Interlocked.Increment(ref starts);
            return true;
        }

        public async Task WaitForStartsAsync(int count)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            while (Starts < count)
            {
                await Task.Delay(5, timeout.Token);
            }
        }

        private int starts;
    }

    private sealed class FakeLog : IRefreshLog
    {
        public Exception? Failure { get; init; }

        public Task RecordAsync(RefreshRun run, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken)
        {
            return Failure is null ? Task.FromResult<RefreshRun?>(null) : Task.FromException<RefreshRun?>(Failure);
        }
    }
}