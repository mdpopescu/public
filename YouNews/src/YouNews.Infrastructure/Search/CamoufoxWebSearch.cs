using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;
using YouNews.Infrastructure.Processes;

namespace YouNews.Infrastructure.Search;

/// <summary>Searches DuckDuckGo through a headless Camoufox browser driven by a small Python script.</summary>
public sealed class CamoufoxWebSearch : IWebSearch
{
    private const string SCRIPT_FILE_NAME = "camoufox_search.py";

    private const string SCRIPT = """
        import asyncio
        import json
        import sys
        from urllib.parse import parse_qs, quote_plus, urlparse

        from camoufox.async_api import AsyncCamoufox
        from playwright.async_api import TimeoutError as PlaywrightTimeoutError


        def unwrap(href):
            if href.startswith("//"):
                href = "https:" + href
            query = parse_qs(urlparse(href).query)
            return query["uddg"][0] if "uddg" in query else href


        async def main(query):
            async with AsyncCamoufox(headless=True) as browser:
                page = await browser.new_page()
                await page.goto("https://html.duckduckgo.com/html/?q=" + quote_plus(query), wait_until="load")
                results = []
                try:
                    await page.wait_for_selector(".result__a", timeout=15000)
                except PlaywrightTimeoutError:
                    print("[]")
                    return
                for item in await page.query_selector_all(".result"):
                    link = await item.query_selector(".result__a")
                    if link is None:
                        continue
                    snippet = await item.query_selector(".result__snippet")
                    results.append({
                        "title": (await link.inner_text()).strip(),
                        "url": unwrap(await link.get_attribute("href") or ""),
                        "snippet": (await snippet.inner_text()).strip() if snippet else "",
                    })
                print(json.dumps(results, ensure_ascii=True))


        asyncio.run(main(sys.argv[1]))
        """;

    private readonly IProcessRunner processRunner;
    private readonly SearchOptions options;
    private readonly string workDirectory;
    private readonly ILogger<CamoufoxWebSearch> logger;
    private readonly SemaphoreSlim scriptLock = new(1, 1);
    private string? scriptPath;

    public CamoufoxWebSearch(IProcessRunner processRunner, IOptions<YouNewsOptions> options, ILogger<CamoufoxWebSearch> logger)
    {
        this.processRunner = processRunner;
        this.options = options.Value.Search;
        workDirectory = options.Value.WorkDirectory;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<WebResult>> SearchAsync(string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        try
        {
            var path = await EnsureScriptAsync(cancellationToken).ConfigureAwait(false);
            var result = await processRunner
                .RunAsync(options.PythonPath, [path, query], workDirectory, TimeSpan.FromSeconds(options.TimeoutSeconds), cancellationToken)
                .ConfigureAwait(false);
            return Parse(result, query);
        }
        catch (TimeoutException)
        {
            logger.LogWarning("Camoufox search for '{Query}' timed out after {Seconds} seconds", query, options.TimeoutSeconds);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Camoufox search for '{Query}' failed", query);
        }

        return [];
    }

    //

    private IReadOnlyList<WebResult> Parse(ProcessResult result, string query)
    {
        if (result.ExitCode != 0)
        {
            logger.LogWarning("Camoufox search for '{Query}' exited with {ExitCode}: {Error}", query, result.ExitCode, result.StandardError);
            return [];
        }

        try
        {
            var items = JsonSerializer.Deserialize<List<ScriptItem>>(result.StandardOutput) ?? [];
            return items
                .Where(it => !string.IsNullOrWhiteSpace(it.Url))
                .Take(options.MaximumResults)
                .Select(it => new WebResult(it.Title ?? "", it.Url!, it.Snippet ?? ""))
                .ToList();
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Camoufox search for '{Query}' printed output that is not valid JSON", query);
            return [];
        }
    }

    private async Task<string> EnsureScriptAsync(CancellationToken cancellationToken)
    {
        await scriptLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (scriptPath is null)
            {
                Directory.CreateDirectory(workDirectory);
                var path = Path.Combine(workDirectory, SCRIPT_FILE_NAME);
                await File.WriteAllTextAsync(path, SCRIPT, cancellationToken).ConfigureAwait(false);
                scriptPath = path;
            }

            return scriptPath;
        }
        finally
        {
            scriptLock.Release();
        }
    }

    private sealed record ScriptItem(
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("snippet")] string? Snippet);
}