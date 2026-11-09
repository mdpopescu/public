using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YouNews.Core.Model;

namespace YouNews.Core.Briefing;

/// <summary>Reads the model's analysis JSON leniently and keeps only values that are valid for this video.</summary>
public sealed class AnalysisParser
{
    public const int MAX_FRAME_TIMES = 2;
    public const int MAX_SEARCH_QUERIES = 3;

    /// <exception cref="FormatException">The response has no JSON object with a headline.</exception>
    public VideoAnalysis Parse(string response, IReadOnlyList<EntryDigest> candidates, Transcript transcript)
    {
        using var document = ReadFirstObject(response);
        var root = document?.RootElement;
        var headline = root is { } element ? ReadStrings(element, "headline").FirstOrDefault() : null;
        if (string.IsNullOrEmpty(headline))
        {
            throw new FormatException("The analysis has no JSON object with a headline. It started with: " + response[..Math.Min(response.Length, MAX_QUOTED_CHARACTERS)]);
        }

        return new VideoAnalysis(
            headline,
            ReadTags(root!.Value),
            ReadFrameTimes(root.Value, transcript),
            ReadRelatedEntryIds(root.Value, candidates),
            ReadStrings(root.Value, "searchQueries").Distinct().Take(MAX_SEARCH_QUERIES).ToList());
    }

    //

    private const int MAX_QUOTED_CHARACTERS = 200;

    private static readonly JsonReaderOptions READER_OPTIONS = new() { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };

    private static readonly Regex BARE_TIMESTAMP = new(@"(?<=[\[,]\s*)\d+(:\d{2}){1,2}(?=\s*[,\]])");

    private static JsonDocument? ReadFirstObject(string response) =>
        ReadFirstObjectExactly(response) ?? ReadFirstObjectExactly(BARE_TIMESTAMP.Replace(response, "\"$&\""));

    private static JsonDocument? ReadFirstObjectExactly(string response)
    {
        for (var start = response.IndexOf('{'); start >= 0; start = response.IndexOf('{', start + 1))
        {
            var document = TryReadValue(Encoding.UTF8.GetBytes(response[start..]));
            if (document is not null)
            {
                return document;
            }
        }

        return null;
    }

    private static JsonDocument? TryReadValue(byte[] json)
    {
        try
        {
            var reader = new Utf8JsonReader(json, READER_OPTIONS);
            return JsonDocument.ParseValue(ref reader);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static List<string> ReadTags(JsonElement root)
    {
        var tags = ReadStrings(root, "tags")
            .Select(tag => EntryTags.ALL.FirstOrDefault(known => known.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            .OfType<string>()
            .Distinct()
            .ToList();

        var onTopic = tags.Where(tag => tag != EntryTags.OFF_TOPIC).ToList();
        return onTopic.Count > 0 ? onTopic : [EntryTags.OFF_TOPIC];
    }

    private static List<TimeSpan> ReadFrameTimes(JsonElement root, Transcript transcript)
    {
        var endSeconds = transcript.Segments.Count > 0 ? transcript.Segments[^1].Start.TotalSeconds : -1;
        return ReadNumbers(root, "frameTimes")
            .Where(seconds => seconds >= 0 && seconds <= endSeconds)
            .DistinctBy(Math.Floor)
            .Take(MAX_FRAME_TIMES)
            .Select(TimeSpan.FromSeconds)
            .ToList();
    }

    private static List<long> ReadRelatedEntryIds(JsonElement root, IReadOnlyList<EntryDigest> candidates)
    {
        var knownIds = candidates.Select(candidate => candidate.Id).ToHashSet();
        return ReadItems(root, "relatedEntryIds")
            .Select(ToWholeNumber)
            .OfType<long>()
            .Where(knownIds.Contains)
            .Distinct()
            .ToList();
    }

    private static long? ToWholeNumber(JsonElement item)
    {
        var parsed = item.ValueKind switch
        {
            JsonValueKind.Number => item.TryGetDecimal(out var decimalValue) ? decimalValue : (decimal?)null,
            JsonValueKind.String => decimal.TryParse(item.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue) ? parsedValue : null,
            _ => null,
        };

        if (parsed is not { } value || value != decimal.Truncate(value) || value < long.MinValue || value > long.MaxValue)
            return null;

        return (long)value;
    }

    private static IEnumerable<string> ReadStrings(JsonElement root, string name) =>
        ReadItems(root, name)
            .Where(item => item.ValueKind == JsonValueKind.String)
            .Select(item => item.GetString()!.Trim())
            .Where(text => text.Length > 0);

    private static IEnumerable<double> ReadNumbers(JsonElement root, string name)
    {
        foreach (var item in ReadItems(root, name))
        {
            if (item.ValueKind == JsonValueKind.Number)
            {
                yield return item.GetDouble();
            }
            else if (item.ValueKind == JsonValueKind.String && TryParseSeconds(item.GetString()!, out var number))
            {
                yield return number;
            }
        }
    }

    private static bool TryParseSeconds(string text, out double seconds)
    {
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
        {
            return true;
        }

        var parsed = TimeSpan.TryParseExact(text.Trim(), ["m\\:ss", "h\\:mm\\:ss"], CultureInfo.InvariantCulture, out var time);
        seconds = time.TotalSeconds;
        return parsed;
    }

    private static IEnumerable<JsonElement> ReadItems(JsonElement root, string name)
    {
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(name, out var value))
        {
            return [];
        }

        return value.ValueKind == JsonValueKind.Array ? value.EnumerateArray() : [value];
    }
}