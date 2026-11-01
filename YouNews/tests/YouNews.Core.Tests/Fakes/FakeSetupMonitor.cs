using YouNews.Core.Model;
using YouNews.Core.Setup;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeSetupMonitor : ISetupMonitor
{
    public SetupSnapshot? Snapshot { get; set; }

    public Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken) => Task.FromResult(Snapshot);
}