using System.Text.RegularExpressions;

namespace YouNews.Core.Briefing;

/// <summary>Removes the Markdown that models add even when told not to. Entries are shown as plain paragraphs.</summary>
public sealed class PlainTextCleaner
{
    public string Clean(string text)
    {
        var cleaned = text.Replace("\r\n", "\n").Replace("**", "");
        cleaned = LEADING_HEADING.Replace(cleaned, "");
        cleaned = LEADING_BULLET.Replace(cleaned, "");
        cleaned = TRAILING_SPACES.Replace(cleaned, "");
        cleaned = EXCESS_BLANK_LINES.Replace(cleaned, "\n\n");
        return cleaned.Trim();
    }

    //

    private static readonly Regex LEADING_HEADING = new(@"^[ \t]*#{1,6}[ \t]+", RegexOptions.Multiline);
    private static readonly Regex LEADING_BULLET = new(@"^[ \t]*[-*][ \t]+", RegexOptions.Multiline);
    private static readonly Regex TRAILING_SPACES = new(@"[ \t]+$", RegexOptions.Multiline);
    private static readonly Regex EXCESS_BLANK_LINES = new(@"\n{3,}");
}