using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Refresh;

/// <summary>Lets only one refresh run at a time, in the background, whoever asked for it.</summary>
public sealed class RefreshCoordinator : IRefreshTrigger, IRefreshStatus, IAsyncDisposable
{
    public RefreshProgress? Current => Volatile.Read(ref current);

    public RefreshCoordinator(IRefreshRun run, IRefreshLog log, TimeProvider time)
    {
        this.run = run;
        this.log = log;
        this.time = time;
    }

    public bool TryStart()
    {
        var starting = new RefreshProgress("Starting", 0, 0);
        if (Interlocked.CompareExchange(ref current, starting, null) is not null)
        {
            return false;
        }

        runningTask = Task.Run(RunAsync);
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        await stopping.CancelAsync().ConfigureAwait(false);
        await runningTask.ConfigureAwait(false);
    }

    //

    private readonly IRefreshRun run;
    private readonly IRefreshLog log;
    private readonly TimeProvider time;
    private readonly CancellationTokenSource stopping = new();

    private RefreshProgress? current;
    private Task runningTask = Task.CompletedTask;

    private async Task RunAsync()
    {
        var startedAt = time.GetUtcNow();
        try
        {
            await run.ExecuteAsync(progress => Volatile.Write(ref current, progress), stopping.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stopping.IsCancellationRequested)
        {
            // The application is closing. Unfinished videos are still pending, so the next run carries on with them.
        }
        catch (Exception exception)
        {
            await RecordCrashAsync(startedAt, exception).ConfigureAwait(false);
        }
        finally
        {
            Volatile.Write(ref current, null);
        }
    }

    private async Task RecordCrashAsync(DateTimeOffset startedAt, Exception exception)
    {
        try
        {
            var crashed = new RefreshRun(startedAt, time.GetUtcNow(), 0, 0, exception.Message);
            await log.RecordAsync(crashed, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // The log itself is what failed. The page will show the previous run, and the next run tries again.
        }
    }
}