using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeRefreshLog : IRefreshLog
{
    public List<RefreshRun> Runs { get; } = [];

    public Task RecordAsync(RefreshRun run, CancellationToken cancellationToken)
    {
        Runs.Add(run);
        return Task.CompletedTask;
    }

    public Task<RefreshRun?> GetLatestAsync(CancellationToken cancellationToken) => Task.FromResult(Runs.LastOrDefault());
}