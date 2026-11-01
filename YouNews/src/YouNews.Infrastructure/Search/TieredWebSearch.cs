using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Search;

/// <summary>Asks the primary search first and falls back to the secondary only when the primary finds too little.</summary>
public sealed class TieredWebSearch : IWebSearch
{
    private readonly IWebSearch primary;
    private readonly IWebSearch secondary;
    private readonly SearchOptions options;

    public TieredWebSearch(IWebSearch primary, IWebSearch secondary, IOptions<YouNewsOptions> options)
    {
        this.primary = primary;
        this.secondary = secondary;
        this.options = options.Value.Search;
    }

    public async Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var results = await primary.SearchAsync(query, cancellationToken).ConfigureAwait(false);
        if (results.Count < options.MinimumResults)
        {
            var more = await secondary.SearchAsync(query, cancellationToken).ConfigureAwait(false);
            results = results.Concat(more).ToList();
        }

        return results
            .DistinctBy(it => Normalize(it.Url))
            .Take(options.MaximumResults)
            .ToList();
    }

    //

    private static string Normalize(string url)
    {
        var withoutScheme = url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? url[8..]
            : url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? url[7..]
            : url;
        return withoutScheme.TrimEnd('/').ToLowerInvariant();
    }
}