using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Search;

/// <summary>Searches through a SearXNG instance's JSON API.</summary>
public sealed class SearxngWebSearch : IWebSearch
{
    public SearxngWebSearch(HttpClient httpClient, IOptions<YouNewsOptions> options, ILogger<SearxngWebSearch> logger)
    {
        this.httpClient = httpClient;
        this.options = options.Value.Search;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));

        try
        {
            return await FetchAsync(query, timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("SearXNG search for '{Query}' timed out after {Seconds} seconds", query, options.TimeoutSeconds);
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or NotSupportedException or UriFormatException)
        {
            logger.LogWarning(ex, "SearXNG search for '{Query}' failed", query);
        }

        return [];
    }

    //

    private readonly HttpClient httpClient;
    private readonly SearchOptions options;
    private readonly ILogger<SearxngWebSearch> logger;

    private async Task<IReadOnlyList<WebResult>> FetchAsync(string query, CancellationToken cancellationToken)
    {
        var uri = $"{options.SearxngUrl.TrimEnd('/')}/search?q={Uri.EscapeDataString(query)}&format=json";
        using var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("SearXNG returned {StatusCode} for '{Query}'", (int)response.StatusCode, query);
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<SearxngResponse>(cancellationToken).ConfigureAwait(false);
        return (body?.Results ?? [])
            .Where(it => !string.IsNullOrWhiteSpace(it.Url))
            .Take(options.MaximumResults)
            .Select(it => new WebResult(it.Title ?? "", it.Url!, it.Content ?? ""))
            .ToList();
    }

    private sealed record SearxngResponse([property: JsonPropertyName("results")] List<SearxngItem>? Results);

    private sealed record SearxngItem(
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("content")] string? Content);
}