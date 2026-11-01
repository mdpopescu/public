using System.Threading.Channels;
using YouNews.Core.Model;

namespace YouNews.Core.Refresh;

public sealed class ReprocessWorker : IReprocessTrigger, IReprocessWorker
{
    public static readonly TimeSpan RETRY_DELAY = TimeSpan.FromSeconds(30);

    public ReprocessWorker(ReprocessDependencies dependencies)
    {
        this.dependencies = dependencies;
    }

    public async Task RequestAsync(long entryId, CancellationToken cancellationToken)
    {
        await dependencies.Queue.RequestAsync(entryId, cancellationToken).ConfigureAwait(false);
        wakeUp.Writer.TryWrite(true);
    }

    public async Task RunAsync(CancellationToken stopping)
    {
        while (!stopping.IsCancellationRequested)
        {
            try
            {
                await WorkOrWaitAsync(stopping).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stopping.IsCancellationRequested)
            {
                return;
            }
            catch (Exception)
            {
                // The queue itself failed, for example because the database is locked.
                await WaitToRetryAsync(stopping).ConfigureAwait(false);
            }
        }
    }

    //

    private readonly ReprocessDependencies dependencies;
    private readonly Channel<bool> wakeUp = System.Threading.Channels.Channel.CreateBounded<bool>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

    private async Task WorkOrWaitAsync(CancellationToken stopping)
    {
        var job = await dependencies.Queue.GetNextAsync(stopping).ConfigureAwait(false);
        if (job is null)
        {
            await wakeUp.Reader.ReadAsync(stopping).ConfigureAwait(false);
            return;
        }

        try
        {
            await ReprocessAsync(job, stopping).ConfigureAwait(false);
        }
        catch (Exception exception) when (!(exception is OperationCanceledException && stopping.IsCancellationRequested))
        {
            await dependencies.Queue.RecordFailureAsync(job.EntryId, exception.Message, stopping).ConfigureAwait(false);
        }
    }

    private async Task ReprocessAsync(ReprocessJob job, CancellationToken stopping)
    {
        var setup = await dependencies.SetupMonitor.GetCurrentAsync(stopping).ConfigureAwait(false);
        var interests = await dependencies.Interests.GetAsync(stopping).ConfigureAwait(false);
        await dependencies.Reprocessor.ReprocessAsync(job, setup, interests, stopping).ConfigureAwait(false);
    }

    private async Task WaitToRetryAsync(CancellationToken stopping)
    {
        try
        {
            await Task.Delay(RETRY_DELAY, dependencies.Time, stopping).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Stopping: the loop condition ends the run.
        }
    }
}