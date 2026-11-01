namespace YouNews.Core.Model;

public sealed record VideoAnalysis(
    string Headline,
    IReadOnlyList<string> Tags,
    IReadOnlyList<TimeSpan> FrameTimes,
    IReadOnlyList<long> RelatedEntryIds,
    IReadOnlyList<string> SearchQueries);

/// <param name="Setup">Null when the setup has never been detected.</param>
public sealed record OpinionRequest(
    string Title,
    string ChannelName,
    Transcript Transcript,
    SetupSnapshot? Setup,
    IReadOnlyList<string> Interests,
    IReadOnlyList<EntryDigest> RelatedEntries,
    IReadOnlyList<WebResult> SearchResults);