namespace YouNews.Core.Refresh;

/// <summary>Writes the queued entries again, one at a time, for as long as the application runs.</summary>
public interface IReprocessWorker
{
    /// <summary>
    /// Works through the queue and then waits for new requests. A failing entry gets its failure recorded and the next one runs.
    /// Completes without throwing when <paramref name="stopping"/> is cancelled, leaving the interrupted request queued.
    /// </summary>
    Task RunAsync(CancellationToken stopping);
}