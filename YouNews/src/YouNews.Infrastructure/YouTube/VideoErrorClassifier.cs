using YouNews.Core.Model;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Decides from yt-dlp error output whether a video is gone for good or worth asking about again.</summary>
public sealed class VideoErrorClassifier
{
    public (VideoAvailability Availability, string Reason) Classify(string standardError, int exitCode)
    {
        var reason = ExtractReason(standardError, exitCode);

        if (ContainsAny(standardError, TRANSIENT_MARKERS))
            return (VideoAvailability.RecheckLater, reason);

        var availability = ContainsAny(standardError, PERMANENT_MARKERS)
            ? VideoAvailability.PermanentlyUnavailable
            : VideoAvailability.RecheckLater;
        return (availability, reason);
    }

    //

    private static readonly string[] TRANSIENT_MARKERS =
    [
        "Sign in to confirm",
        "Premieres in",
        "This live event will begin",
    ];

    private static readonly string[] PERMANENT_MARKERS =
    [
        "members-only",
        "Join this channel",
        "Private video",
        "Video unavailable",
        "has been removed",
        "removed by the uploader",
        "been deleted",
        "account associated with this video has been terminated",
    ];

    private static bool ContainsAny(string text, string[] markers) =>
        markers.Any(marker => text.Contains(marker, StringComparison.OrdinalIgnoreCase));

    private static string ExtractReason(string standardError, int exitCode)
    {
        var lines = standardError.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var error = lines.LastOrDefault(line => line.StartsWith("ERROR:", StringComparison.Ordinal));
        if (error == null)
            return lines.Length > 0 ? lines[^1] : $"yt-dlp failed with exit code {exitCode}.";

        // "ERROR: [youtube] ZerTreaAgPA: Join this channel ..." becomes "Join this channel ..."
        var text = error["ERROR:".Length..].Trim();
        if (text.StartsWith('['))
            text = text[(text.IndexOf(']') + 1)..].Trim();
        else
            return text;

        var idEnd = text.IndexOf(": ", StringComparison.Ordinal);
        return idEnd is >= 0 and < 30 ? text[(idEnd + 2)..].Trim() : text;
    }
}