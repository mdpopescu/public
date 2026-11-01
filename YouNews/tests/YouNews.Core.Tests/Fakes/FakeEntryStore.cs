using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeEntryStore : IEntryStore
{
    public List<NewEntry> Added { get; } = [];

    public List<EntryDigest> Digests { get; } = [];

    public List<EntryRevision> Revised { get; } = [];

    public Action<long>? OnAdded { get; set; }

    public Task<long> AddAsync(NewEntry entry, CancellationToken cancellationToken)
    {
        Added.Add(entry);
        OnAdded?.Invoke(entry.VideoId);
        return Task.FromResult((long)Added.Count);
    }

    public Task ReviseAsync(EntryRevision revision, CancellationToken cancellationToken)
    {
        Revised.Add(revision);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EntryDigest>> ListRecentDigestsAsync(int limit, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<EntryDigest>>(Digests);
}