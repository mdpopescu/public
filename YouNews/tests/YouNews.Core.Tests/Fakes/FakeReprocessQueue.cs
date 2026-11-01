using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeReprocessQueue : IReprocessQueue
{
    public Dictionary<long, string> Failures { get; } = [];

    public int GetNextCalls { get; private set; }

    /// <summary>The number of GetNextAsync calls that fail, as a locked database does.</summary>
    public int FailingCalls { get; set; }

    public Action? OnGetNextFailed { get; set; }

    public List<long> Queued
    {
        get
        {
            lock (gate)
            {
                return [.. requests];
            }
        }
    }

    public Task RequestAsync(long entryId, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            if (!requests.Contains(entryId))
                requests.Add(entryId);

            Failures.Remove(entryId);
        }

        return Task.CompletedTask;
    }

    public Task<ReprocessJob?> GetNextAsync(CancellationToken cancellationToken)
    {
        lock (gate)
        {
            GetNextCalls++;
            if (GetNextCalls <= FailingCalls)
            {
                OnGetNextFailed?.Invoke();
                throw new InvalidOperationException("database is locked");
            }

            var next = requests.Where(it => !Failures.ContainsKey(it)).Select(it => (long?)it).FirstOrDefault();
            return Task.FromResult(next is { } id ? new ReprocessJob(id, $"video{id}", $"Title {id}", "Channel One") : null);
        }
    }

    public Task RecordFailureAsync(long entryId, string reason, CancellationToken cancellationToken)
    {
        lock (gate)
        {
            Failures[entryId] = reason;
        }

        return Task.CompletedTask;
    }

    public void Complete(long entryId)
    {
        lock (gate)
        {
            requests.Remove(entryId);
        }
    }

    //

    private readonly Lock gate = new();
    private readonly List<long> requests = [];
}