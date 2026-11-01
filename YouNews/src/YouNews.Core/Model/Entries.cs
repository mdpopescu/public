namespace YouNews.Core.Model;

public static class EntryTags
{
    public const string LOCAL_MODELS = "local models";
    public const string RESEARCH = "research";
    public const string LEGISLATION = "legislation";
    public const string HARDWARE_MARKET = "hardware market";
    public const string INDUSTRY = "industry";
    public const string OFF_TOPIC = "off-topic";

    public static readonly IReadOnlyList<string> ALL = [LOCAL_MODELS, RESEARCH, LEGISLATION, HARDWARE_MARKET, INDUSTRY, OFF_TOPIC];
}

public enum ImageKind
{
    // ReSharper disable InconsistentNaming
    // These names are stored as text in SQLite.
    Thumbnail,
    Frame,
    // ReSharper restore InconsistentNaming
}

/// <param name="RelativePath">Path below the images folder, with forward slashes.</param>
public sealed record EntryImage(string RelativePath, ImageKind Kind);

public sealed record WebSource(string Title, string Url);

/// <summary>A finished entry ready to be stored. Storing it also marks its video as done.</summary>
public sealed record NewEntry(
    long VideoId,
    DateOnly Day,
    string Headline,
    IReadOnlyList<string> Tags,
    string Summary,
    string Opinion,
    IReadOnlyList<WebSource> WebSources,
    IReadOnlyList<EntryImage> Images,
    IReadOnlyList<long> RelatedEntryIds);

/// <summary>A compact form of an earlier entry, used as context for new ones.</summary>
public sealed record EntryDigest(long Id, DateOnly Day, string Headline, IReadOnlyList<string> Tags, string Summary);

public sealed record RelatedLink(long EntryId, DateOnly Day, string Headline);

/// <summary>An entry as shown on a day page.</summary>
public sealed record Entry(
    long Id,
    DateOnly Day,
    string ChannelName,
    string VideoYouTubeId,
    string Headline,
    IReadOnlyList<string> Tags,
    string Summary,
    string Opinion,
    IReadOnlyList<WebSource> WebSources,
    IReadOnlyList<EntryImage> Images,
    IReadOnlyList<RelatedLink> Related,
    bool IsReprocessQueued = false,
    string? ReprocessFailure = null);

/// <summary>An entry waiting to be written again with the current model.</summary>
public sealed record ReprocessJob(long EntryId, string YouTubeId, string Title, string ChannelName);

/// <summary>The new texts and links of an existing entry. Headline, tags, images and day stay as they are.</summary>
public sealed record EntryRevision(
    long EntryId,
    string Summary,
    string Opinion,
    IReadOnlyList<WebSource> WebSources,
    IReadOnlyList<long> RelatedEntryIds);

public sealed record DayCount(DateOnly Day, int EntryCount);