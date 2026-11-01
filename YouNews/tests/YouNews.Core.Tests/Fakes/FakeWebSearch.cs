using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Core.Tests.Fakes;

internal sealed class FakeWebSearch : IWebSearch
{
    public List<WebResult> Results { get; } = [];

    public List<string> Queries { get; } = [];

    public Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        Queries.Add(query);
        return Task.FromResult<IReadOnlyList<WebResult>>(Results);
    }
}