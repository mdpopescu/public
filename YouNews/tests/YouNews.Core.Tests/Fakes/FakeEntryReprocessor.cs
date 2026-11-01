using YouNews.Core.Model;
using YouNews.Core.Refresh;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeEntryReprocessor(FakeReprocessQueue queue) : IEntryReprocessor
{
    public List<long> Processed { get; } = [];

    public List<SetupSnapshot?> Setups { get; } = [];

    /// <summary>What the reprocess does; by default it succeeds at once.</summary>
    public Func<ReprocessJob, CancellationToken, Task> Handler { get; set; } = (_, _) => Task.CompletedTask;

    public async Task ReprocessAsync(ReprocessJob job, SetupSnapshot? setup, IReadOnlyList<string> interests, CancellationToken cancellationToken)
    {
        Setups.Add(setup);
        await Handler(job, cancellationToken).ConfigureAwait(false);
        queue.Complete(job.EntryId);
        Processed.Add(job.EntryId);
    }
}