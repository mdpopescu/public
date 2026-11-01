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
        await CatchUpAsync(stoppingToken);

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

    private async Task CatchUpAsync(CancellationToken stoppingToken)
    {
        try
        {
            var latestRun = await log.GetLatestAsync(stoppingToken);
            if (schedule.IsOverdue(time.GetUtcNow(), latestRun))
            {
                trigger.TryStart();
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // An exception here would stop the whole service. Missing one catch-up is much the lesser evil.
        }
    }
}