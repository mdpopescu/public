using System.Text.RegularExpressions;

namespace YouNews.Web.Presentation;

/// <summary>Cuts model-written text into paragraphs, so that a long block never becomes a wall of text.</summary>
public sealed partial class ParagraphSplitter
{
    public IReadOnlyList<string> Split(string text)
    {
        var blocks = BlankLine().Split(text.Replace("\r\n", "\n"))
            .Select(block => block.Trim())
            .Where(block => block.Length > 0)
            .ToList();

        if (blocks.Count == 1)
            blocks = blocks[0].Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToList();

        return blocks.SelectMany(BreakUpLong).ToList();
    }

    //

    private const int LONG_PARAGRAPH_LENGTH = 700;
    private const int TARGET_PARAGRAPH_LENGTH = 450;

    [GeneratedRegex(@"\n\s*\n")]
    private static partial Regex BlankLine();

    [GeneratedRegex("(?<=[.!?])\\s+(?=[A-Z0-9\"'(])")]
    private static partial Regex SentenceBoundary();

    private static IEnumerable<string> BreakUpLong(string paragraph)
    {
        if (paragraph.Length <= LONG_PARAGRAPH_LENGTH)
        {
            yield return paragraph;
            yield break;
        }

        var current = "";
        foreach (var sentence in SentenceBoundary().Split(paragraph))
        {
            if (current.Length > 0 && current.Length + sentence.Length > TARGET_PARAGRAPH_LENGTH)
            {
                yield return current;
                current = "";
            }

            current = current.Length == 0 ? sentence : current + " " + sentence;
        }

        if (current.Length > 0)
            yield return current;
    }
}