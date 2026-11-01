using YouNews.Core.Model;

namespace YouNews.Core.Briefing;

/// <summary>Splits a long transcript into overlapping parts that keep their timestamps. No words are dropped.</summary>
public sealed class TranscriptChunker
{
    public const int CHUNK_WORDS = 12000;
    public const int OVERLAP_WORDS = 500;

    public IReadOnlyList<Transcript> Split(Transcript transcript)
    {
        var pieces = transcript.Segments.SelectMany(SplitLongSegment).ToList();
        var chunks = new List<Transcript>();
        var start = 0;
        while (start < pieces.Count)
        {
            var end = FindChunkEnd(pieces, start);
            chunks.Add(new Transcript(pieces.GetRange(start, end - start)));
            if (end == pieces.Count)
            {
                break;
            }

            start = FindOverlapStart(pieces, start, end);
        }

        return chunks;
    }

    //

    private static int CountWords(TranscriptSegment segment) =>
        segment.Text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;

    private static IEnumerable<TranscriptSegment> SplitLongSegment(TranscriptSegment segment)
    {
        if (CountWords(segment) <= OVERLAP_WORDS)
        {
            return [segment];
        }

        return segment.Text
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            .Chunk(OVERLAP_WORDS)
            .Select(words => new TranscriptSegment(segment.Start, string.Join(' ', words)));
    }

    private static int FindChunkEnd(List<TranscriptSegment> pieces, int start)
    {
        var words = 0;
        var end = start;
        while (end < pieces.Count && words + CountWords(pieces[end]) <= CHUNK_WORDS)
        {
            words += CountWords(pieces[end]);
            end++;
        }

        return end;
    }

    private static int FindOverlapStart(List<TranscriptSegment> pieces, int chunkStart, int chunkEnd)
    {
        var words = 0;
        var overlapStart = chunkEnd;
        while (overlapStart - 1 > chunkStart && words + CountWords(pieces[overlapStart - 1]) <= OVERLAP_WORDS)
        {
            words += CountWords(pieces[overlapStart - 1]);
            overlapStart--;
        }

        return overlapStart;
    }
}