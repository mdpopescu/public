using System.Text.Json;
using Microsoft.Extensions.Options;
using YouNews.Core.Model;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Downloads the English json3 caption files of a video with yt-dlp and reads them.</summary>
public sealed class YtDlpCaptionSource : ICaptionSource
{
    public YtDlpCaptionSource(YtDlpRunner ytDlp, Json3Parser parser, IOptions<YouNewsOptions> options)
    {
        this.ytDlp = ytDlp;
        this.parser = parser;
        workDirectory = options.Value.WorkDirectory;
    }

    public async Task<Transcript?> GetCaptionsAsync(string youTubeId, CancellationToken cancellationToken)
    {
        var url = ytDlp.WatchUrl(youTubeId);
        using var folder = new ScratchFolder(workDirectory);
        var template = Path.Combine(folder.Path, $"{youTubeId}.%(ext)s");

        var result = await ytDlp.RunAsync(
            ["--skip-download", "--write-subs", "--write-auto-subs", "--write-info-json", "--sub-langs", "en.*,en", "--sub-format", "json3", "-o", template, url],
            cancellationToken).ConfigureAwait(false);

        var captionFiles = Directory.GetFiles(folder.Path, $"{youTubeId}.*.json3");
        if (captionFiles.Length == 0)
            return result.ExitCode == 0
                ? null
                : throw new InvalidOperationException($"yt-dlp could not get captions for {youTubeId}: {result.StandardError.Trim()}");

        var manualLanguages = await ReadManualLanguagesAsync(folder.Path, youTubeId, cancellationToken).ConfigureAwait(false);
        var best = captionFiles.OrderBy(file => Rank(file, youTubeId, manualLanguages)).First();
        var json = await File.ReadAllTextAsync(best, cancellationToken).ConfigureAwait(false);
        var segments = parser.Parse(json);
        return segments.Count == 0 ? null : new Transcript(segments);
    }

    //

    private readonly YtDlpRunner ytDlp;
    private readonly Json3Parser parser;
    private readonly string workDirectory;

    private static string LanguageOf(string file, string youTubeId)
    {
        var name = Path.GetFileName(file);
        return name[(youTubeId.Length + 1)..^".json3".Length];
    }

    private static int Rank(string file, string youTubeId, ISet<string> manualLanguages)
    {
        var language = LanguageOf(file, youTubeId);
        if (manualLanguages.Contains(language))
            return 0;

        return language switch
        {
            "en-orig" => 1,
            "en" => 2,
            _ => 3,
        };
    }

    private static async Task<ISet<string>> ReadManualLanguagesAsync(string folder, string youTubeId, CancellationToken cancellationToken)
    {
        var infoFile = Path.Combine(folder, $"{youTubeId}.info.json");
        if (!File.Exists(infoFile))
            return new HashSet<string>();

        var json = await File.ReadAllTextAsync(infoFile, cancellationToken).ConfigureAwait(false);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.TryGetProperty("subtitles", out var subtitles) && subtitles.ValueKind == JsonValueKind.Object
            ? subtitles.EnumerateObject().Select(property => property.Name).ToHashSet()
            : new HashSet<string>();
    }
}