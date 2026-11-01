using YouNews.Core.Briefing;
using YouNews.Core.Model;

namespace YouNews.Core.Tests.Briefing;

[TestClass]
public sealed class TranscriptChunkerTests
{
    [TestMethod]
    public void TranscriptThatFitsStaysInOneChunk()
    {
        var sut = new TranscriptChunker();
        var transcript = TranscriptBuilder.WithWords(TranscriptChunker.CHUNK_WORDS);

        var chunks = sut.Split(transcript);

        Assert.HasCount(1, chunks);
        Assert.AreEqual(TranscriptChunker.CHUNK_WORDS, chunks[0].WordCount);
    }

    [TestMethod]
    public void OneWordMoreStartsASecondOverlappingChunk()
    {
        var sut = new TranscriptChunker();
        var transcript = TranscriptBuilder.WithWords(TranscriptChunker.CHUNK_WORDS + 1, wordsPerSegment: 1);

        var chunks = sut.Split(transcript);

        Assert.HasCount(2, chunks);
        Assert.AreEqual(TranscriptChunker.CHUNK_WORDS, chunks[0].WordCount);
        Assert.AreEqual(TranscriptChunker.OVERLAP_WORDS + 1, chunks[1].WordCount);
    }

    [TestMethod]
    public void ChunksKeepEveryWordInOrderWithTheirTimestampsAndOverlap()
    {
        var sut = new TranscriptChunker();
        var transcript = TranscriptBuilder.WithWords(100000);

        var chunks = sut.Split(transcript);

        Assert.IsGreaterThan(8, chunks.Count);
        Assert.IsTrue(chunks.All(chunk => chunk.WordCount <= TranscriptChunker.CHUNK_WORDS), "A chunk is larger than the limit.");
        var allSegments = chunks.SelectMany(chunk => chunk.Segments).Distinct().ToList();
        CollectionAssert.AreEqual(transcript.Segments.ToList(), allSegments, "Chunks must cover the transcript exactly, in order, with original timestamps.");
        for (var index = 1; index < chunks.Count; index++)
        {
            var overlap = chunks[index - 1].Segments.Intersect(chunks[index].Segments).ToList();
            var overlapWords = new Transcript(overlap).WordCount;
            Assert.IsInRange(1, TranscriptChunker.OVERLAP_WORDS, overlapWords, $"Overlap between chunks {index - 1} and {index}");
        }
    }

    [TestMethod]
    public void HugeSegmentIsCutSoNothingIsLostAndChunksStillOverlap()
    {
        var sut = new TranscriptChunker();
        var words = Enumerable.Range(0, 30000).Select(index => "w" + index);
        var start = TimeSpan.FromMinutes(5);
        var transcript = new Transcript([new TranscriptSegment(start, string.Join(' ', words))]);

        var chunks = sut.Split(transcript);

        Assert.IsGreaterThan(1, chunks.Count);
        Assert.IsTrue(chunks.All(chunk => chunk.WordCount <= TranscriptChunker.CHUNK_WORDS));
        Assert.IsTrue(chunks.SelectMany(chunk => chunk.Segments).All(segment => segment.Start == start), "The pieces keep the original start time.");
        var seen = chunks.SelectMany(chunk => chunk.Segments).SelectMany(segment => segment.Text.Split(' ')).ToHashSet();
        Assert.HasCount(30000, seen, "Every word must appear in some chunk.");
        Assert.IsNotEmpty(chunks[0].Segments.Intersect(chunks[1].Segments).ToList(), "Neighbouring chunks must overlap.");
    }

    [TestMethod]
    public void EmptyTranscriptHasNoChunks()
    {
        var sut = new TranscriptChunker();

        Assert.IsEmpty(sut.Split(new Transcript([])));
    }
}