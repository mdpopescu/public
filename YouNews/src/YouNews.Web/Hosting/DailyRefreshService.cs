using YouNews.Core.Ports;
using YouNews.Core.Refresh;

namespace YouNews.Web.Hosting;

/// <summary>Service mode: refreshes every day at the scheduled time, catching up after a missed one.</summary>
public sealed class DailyRefreshService : BackgroundService
{
    public DailyRefreshService(IRefreshTrigger trigger, IRefreshLog log, RefreshSchedule schedule, TimeProvider time)
    {
        this.trigger = trigger;
        this.log = log;
        this.schedule = schedule;
        this.time = time;
    }

    //

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var latestRun = await log.GetLatestAsync(stoppingToken);
        if (schedule.IsOverdue(time.GetUtcNow(), latestRun))
        {
            trigger.TryStart();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(schedule.DelayUntilNextRun(time.GetUtcNow()), time, stoppingToken);
            trigger.TryStart();
        }
    }

    //

    private readonly IRefreshTrigger trigger;
    private readonly IRefreshLog log;
    private readonly RefreshSchedule schedule;
    private readonly TimeProvider time;
}