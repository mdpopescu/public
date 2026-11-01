using System.Text.Json;
using System.Text.RegularExpressions;
using YouNews.Core.Model;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Turns a yt-dlp json3 caption file into transcript segments.</summary>
public sealed partial class Json3Parser
{
    public IReadOnlyList<TranscriptSegment> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("events", out var events))
            return [];

        return events.EnumerateArray()
            .Select(ToSegment)
            .OfType<TranscriptSegment>()
            .ToList();
    }

    //

    private static TranscriptSegment? ToSegment(JsonElement caption)
    {
        if (!caption.TryGetProperty("segs", out var segments))
            return null;

        var text = string.Concat(segments.EnumerateArray().Select(ReadText));
        text = WhitespacePattern().Replace(text, " ").Trim();
        if (text.Length == 0)
            return null;

        var startMilliseconds = caption.TryGetProperty("tStartMs", out var start) ? start.GetInt64() : 0;
        return new TranscriptSegment(TimeSpan.FromMilliseconds(startMilliseconds), text);
    }

    private static string ReadText(JsonElement segment) =>
        segment.TryGetProperty("utf8", out var text) ? text.GetString() ?? "" : "";

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespacePattern();
}