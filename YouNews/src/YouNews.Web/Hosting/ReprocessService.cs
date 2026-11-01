using YouNews.Core.Refresh;

namespace YouNews.Web.Hosting;

/// <summary>Runs the reprocess worker for as long as the application runs.</summary>
public sealed class ReprocessService : BackgroundService
{
    public ReprocessService(IReprocessWorker worker)
    {
        this.worker = worker;
    }

    //

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => worker.RunAsync(stoppingToken);

    //

    private readonly IReprocessWorker worker;
}