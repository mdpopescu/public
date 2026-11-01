using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeInterestStore : IInterestStore
{
    public Task<IReadOnlyList<string>> GetAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<string>>(["local models"]);

    public Task SaveAsync(IReadOnlyList<string> interests, CancellationToken cancellationToken) => throw new NotSupportedException();
}