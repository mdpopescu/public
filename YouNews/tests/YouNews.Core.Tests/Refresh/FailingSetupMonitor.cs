using YouNews.Core.Model;
using YouNews.Core.Setup;

namespace YouNews.Core.Tests.Refresh;

internal sealed class FailingSetupMonitor : ISetupMonitor
{
    public Task<SetupSnapshot?> GetCurrentAsync(CancellationToken cancellationToken) => throw new InvalidOperationException("setup unavailable");
}