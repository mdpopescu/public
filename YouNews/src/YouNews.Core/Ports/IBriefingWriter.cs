using YouNews.Core.Model;

namespace YouNews.Core.Ports;

/// <summary>Writes the text of an entry from a video's transcript.</summary>
public interface IBriefingWriter
{
    /// <summary>Works out the headline, tags, frame times, related entries and search queries for a video.</summary>
    /// <param name="title">The video's title.</param>
    /// <param name="channelName">The name of the channel that published the video.</param>
    /// <param name="transcript">The video's transcript.</param>
    /// <param name="candidates">Earlier entries that may be related. Returned related IDs always come from this list.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    Task<VideoAnalysis> AnalyzeAsync(string title, string channelName, Transcript transcript, IReadOnlyList<EntryDigest> candidates, CancellationToken cancellationToken);

    /// <summary>Writes a summary whose length is proportional to the transcript's.</summary>
    /// <param name="title">The video's title.</param>
    /// <param name="transcript">The video's transcript.</param>
    /// <param name="cancellationToken">Cancels the writing.</param>
    Task<string> SummarizeAsync(string title, Transcript transcript, CancellationToken cancellationToken);

    /// <summary>Writes the multi-paragraph expert opinion.</summary>
    /// <param name="request">The video, its analysis, summary and search results the opinion is based on.</param>
    /// <param name="cancellationToken">Cancels the writing.</param>
    Task<string> WriteOpinionAsync(OpinionRequest request, CancellationToken cancellationToken);
}