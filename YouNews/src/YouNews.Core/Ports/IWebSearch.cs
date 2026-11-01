using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Searches the web.</summary>
public interface IWebSearch
{
    /// <summary>Gets results for a query. A search that cannot be performed gives an empty list rather than an error.</summary>
    Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken);
}