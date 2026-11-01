using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Stores finished entries and supplies earlier ones as context for new ones.</summary>
public interface IEntryStore
{
    /// <summary>Stores an entry and marks its video as done, as one atomic change.</summary>
    Task<long> AddAsync(NewEntry entry, CancellationToken cancellationToken);

    /// <summary>
    /// Replaces the summary, opinion, web sources and related entries of an entry and removes its reprocess request,
    /// as one atomic change. When it fails, the entry and the request stay as they were.
    /// </summary>
    Task ReviseAsync(EntryRevision revision, CancellationToken cancellationToken);

    /// <summary>Gets the most recent entries in compact form, newest first.</summary>
    Task<IReadOnlyList<EntryDigest>> ListRecentDigestsAsync(int limit, CancellationToken cancellationToken);
}