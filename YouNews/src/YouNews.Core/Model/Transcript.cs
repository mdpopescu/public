namespace YouNews.Core.Model;

public sealed record TranscriptSegment(TimeSpan Start, string Text);

public sealed record Transcript(IReadOnlyList<TranscriptSegment> Segments)
{
    public int WordCount => Segments.Sum(segment => segment.Text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length);
}