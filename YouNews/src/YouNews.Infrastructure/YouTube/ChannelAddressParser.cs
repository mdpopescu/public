using System.Text.RegularExpressions;

namespace YouNews.Infrastructure.YouTube;

/// <summary>Turns what a user typed into a YouTube channel address, or rejects it.</summary>
public sealed partial class ChannelAddressParser
{
    /// <summary>Gets the address to give to yt-dlp, or null when the text is clearly not a YouTube channel.</summary>
    public string? ToChannelUrl(string text)
    {
        var trimmed = text.Trim();
        if (trimmed.StartsWith('@') && HandlePattern().IsMatch(trimmed[1..]))
            return $"https://www.youtube.com/{trimmed}";

        if (BareNamePattern().IsMatch(trimmed))
            return trimmed.StartsWith("UC", StringComparison.Ordinal) && trimmed.Length == 24
                ? $"https://www.youtube.com/channel/{trimmed}"
                : $"https://www.youtube.com/@{trimmed}";

        return ParseUrl(trimmed);
    }

    //

    [GeneratedRegex(@"^[\p{L}\p{N}_.\-]{1,100}$")]
    private static partial Regex HandlePattern();

    [GeneratedRegex(@"^[\p{L}\p{N}_\-]{1,100}$")]
    private static partial Regex BareNamePattern();

    private static string? ParseUrl(string text)
    {
        var candidate = text.Contains("://", StringComparison.Ordinal) ? text : $"https://{text}";
        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            return null;

        var isYouTube = uri.Host == "youtube.com" || uri.Host.EndsWith(".youtube.com", StringComparison.Ordinal);
        return isYouTube && uri.AbsolutePath.Trim('/').Length > 0 ? $"https://www.youtube.com{uri.AbsolutePath.TrimEnd('/')}" : null;
    }
}