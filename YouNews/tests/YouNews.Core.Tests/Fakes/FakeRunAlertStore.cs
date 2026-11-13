using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeRunAlertStore : IRunAlertStore
{
    public RunAlert? Current { get; private set; }

    public int Raised { get; private set; }

    public Task RaiseAsync(string message, DateTimeOffset raisedAt, CancellationToken cancellationToken)
    {
        Raised++;
        Current = new RunAlert(message, raisedAt);
        return Task.CompletedTask;
    }

    public Task<RunAlert?> GetAsync(CancellationToken cancellationToken) => Task.FromResult(Current);

    public Task DismissAsync(CancellationToken cancellationToken)
    {
        Current = null;
        return Task.CompletedTask;
    }
}