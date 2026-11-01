using YouNews.Core.Model;
using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Refresh;

[TestClass]
public sealed class RefreshScheduleTests
{
    [TestMethod]
    public void BeforeTheTimeRunsTheSameDay()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);

        var delay = sut.DelayUntilNextRun(new DateTimeOffset(2026, 11, 1, 5, 0, 0, TimeSpan.Zero));

        Assert.AreEqual(TimeSpan.FromHours(1), delay);
    }

    [TestMethod]
    public void ExactlyAtTheTimeWaitsForTomorrow()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);

        var delay = sut.DelayUntilNextRun(new DateTimeOffset(2026, 11, 1, 6, 0, 0, TimeSpan.Zero));

        Assert.AreEqual(TimeSpan.FromHours(24), delay);
    }

    [TestMethod]
    public void AutumnClockChangeDayIsTwentyFiveHoursLong()
    {
        var sut = new RefreshSchedule(SIX, Bucharest());

        var justAfterSixOnSaturday = new DateTimeOffset(2026, 10, 24, 6, 0, 0, TimeSpan.FromHours(3));
        var delay = sut.DelayUntilNextRun(justAfterSixOnSaturday);

        Assert.AreEqual(TimeSpan.FromHours(25), delay, "Clocks go back on the 25th, and the run must still be at 06:00 local.");
    }

    [TestMethod]
    public void SpringClockChangeDayIsTwentyThreeHoursLong()
    {
        var sut = new RefreshSchedule(SIX, Bucharest());

        var delay = sut.DelayUntilNextRun(new DateTimeOffset(2026, 3, 28, 6, 0, 0, TimeSpan.FromHours(2)));

        Assert.AreEqual(TimeSpan.FromHours(23), delay);
    }

    [TestMethod]
    public void ATimeTheClocksSkipRunsJustAfterTheJump()
    {
        var sut = new RefreshSchedule(new TimeOnly(3, 30), Bucharest());

        var delay = sut.DelayUntilNextRun(new DateTimeOffset(2026, 3, 28, 12, 0, 0, TimeSpan.FromHours(2)));

        var expected = new DateTimeOffset(2026, 3, 29, 4, 30, 0, TimeSpan.FromHours(3)) - new DateTimeOffset(2026, 3, 28, 12, 0, 0, TimeSpan.FromHours(2));
        Assert.AreEqual(expected, delay);
    }

    [TestMethod]
    public void OverdueWhenAfterTheTimeAndNoRunToday()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);
        var yesterday = RunStartedAt(new DateTimeOffset(2026, 10, 31, 6, 0, 0, TimeSpan.Zero));

        Assert.IsTrue(sut.IsOverdue(new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero), yesterday));
    }

    [TestMethod]
    public void OverdueOnAVeryFirstStartAfterTheTime()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);

        Assert.IsTrue(sut.IsOverdue(new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero), null));
    }

    [TestMethod]
    public void NotOverdueBeforeTheTime()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);

        Assert.IsFalse(sut.IsOverdue(new DateTimeOffset(2026, 11, 1, 5, 59, 0, TimeSpan.Zero), null));
    }

    [TestMethod]
    public void NotOverdueWhenAlreadyRunToday()
    {
        var sut = new RefreshSchedule(SIX, TimeZoneInfo.Utc);
        var today = RunStartedAt(new DateTimeOffset(2026, 11, 1, 6, 0, 1, TimeSpan.Zero));

        Assert.IsFalse(sut.IsOverdue(new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero), today));
    }

    //

    private static readonly TimeOnly SIX = new(6, 0);

    private static TimeZoneInfo Bucharest() => TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");

    private static RefreshRun RunStartedAt(DateTimeOffset startedAt) => new(startedAt, startedAt, 0, 0, null);
}