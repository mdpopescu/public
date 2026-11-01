namespace YouNews.Core.Refresh;

/// <summary>Asks for an entry to be written again.</summary>
public interface IReprocessTrigger
{
    /// <summary>Queues the entry and wakes the worker. Does nothing for an unknown entry.</summary>
    Task RequestAsync(long entryId, CancellationToken cancellationToken);
}