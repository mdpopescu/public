using YouNews.Core.Refresh;

namespace YouNews.Web.Hosting;

/// <summary>Command-line mode: refreshes once each time the application is launched.</summary>
public sealed class RefreshOnLaunchService : IHostedService
{
    public RefreshOnLaunchService(IRefreshTrigger trigger)
    {
        this.trigger = trigger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        trigger.TryStart();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    //

    private readonly IRefreshTrigger trigger;
}