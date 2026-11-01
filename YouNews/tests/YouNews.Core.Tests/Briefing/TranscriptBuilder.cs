using YouNews.Core.Model;

namespace YouNews.Core.Tests.Briefing;

public static class TranscriptBuilder
{
    /// <summary>Builds a transcript of unique words ("w0", "w1", ...), with one segment every 4 seconds.</summary>
    public static Transcript WithWords(int wordCount, int wordsPerSegment = 10)
    {
        var segments = Enumerable.Range(0, wordCount)
            .Select(index => "w" + index)
            .Chunk(wordsPerSegment)
            .Select((words, index) => new TranscriptSegment(TimeSpan.FromSeconds(index * 4), string.Join(' ', words)))
            .ToList();

        return new Transcript(segments);
    }
}