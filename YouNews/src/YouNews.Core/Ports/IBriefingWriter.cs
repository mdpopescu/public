using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Writes the text of an entry from a video's transcript.</summary>
public interface IBriefingWriter
{
    /// <summary>Works out the headline, tags, frame times, related entries and search queries for a video.</summary>
    /// <param name="candidates">Earlier entries that may be related. Returned related IDs always come from this list.</param>
    Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken);

    /// <summary>Writes a summary whose length is proportional to the transcript's.</summary>
    Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken);

    /// <summary>Writes the multi-paragraph expert opinion.</summary>
    Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken);
}