using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>The entries waiting to be written again, as the worker that does it sees them.</summary>
public interface IReprocessQueue
{
    /// <summary>Queues an entry, or clears the failure of one already requested. Does nothing for an unknown entry.</summary>
    Task RequestAsync(long entryId, CancellationToken cancellationToken);

    /// <summary>Gets the oldest request that has not failed, or null when there is none.</summary>
    Task<ReprocessJob?> GetNextAsync(CancellationToken cancellationToken);

    /// <summary>Keeps the reason an entry could not be written again; the request stays until it is asked for again.</summary>
    Task RecordFailureAsync(long entryId, string reason, CancellationToken cancellationToken);
}