using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

/// <summary>Decides when the daily refresh should happen, in the machine's local time.</summary>
public sealed class RefreshSchedule
{
    public RefreshSchedule(TimeOnly dailyAt, TimeZoneInfo zone)
    {
        this.dailyAt = dailyAt;
        this.zone = zone;
    }

    /// <summary>Tells whether today's run was missed, for example because the machine was off at the scheduled time.</summary>
    public bool IsOverdue(DateTimeOffset now, RefreshRun? latestRun)
    {
        var localNow = TimeZoneInfo.ConvertTime(now, zone);
        if (TimeOnly.FromDateTime(localNow.DateTime) < dailyAt)
        {
            return false;
        }

        return latestRun is null || LocalDate(latestRun.StartedAt) != DateOnly.FromDateTime(localNow.DateTime);
    }

    public TimeSpan DelayUntilNextRun(DateTimeOffset now)
    {
        var localNow = TimeZoneInfo.ConvertTime(now, zone);
        var today = DateOnly.FromDateTime(localNow.DateTime);
        var nextRun = ToInstant(today);
        if (nextRun <= now)
        {
            nextRun = ToInstant(today.AddDays(1));
        }

        return nextRun - now;
    }

    //

    private readonly TimeOnly dailyAt;
    private readonly TimeZoneInfo zone;

    private DateOnly LocalDate(DateTimeOffset instant) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(instant, zone).DateTime);

    private DateTimeOffset ToInstant(DateOnly date)
    {
        var local = date.ToDateTime(dailyAt, DateTimeKind.Unspecified);
        if (zone.IsInvalidTime(local))
        {
            // The clocks jumped over this time, so run just after the jump.
            local = local.AddHours(1);
        }

        var offset = zone.IsAmbiguousTime(local) ? zone.GetAmbiguousTimeOffsets(local).Max() : zone.GetUtcOffset(local);
        return new DateTimeOffset(local, offset);
    }
}