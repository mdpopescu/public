namespace YouNews.Core.Model;

public enum VideoAvailability
{
    // ReSharper disable InconsistentNaming
    // Named like VideoState, whose names are stored as text in SQLite.
    Eligible,
    PermanentlyUnavailable,
    RecheckLater,
    // ReSharper restore InconsistentNaming
}

public enum VideoState
{
    // ReSharper disable InconsistentNaming
    // These names are stored as text in SQLite.
    Pending,
    Recheck,
    Skipped,
    GaveUp,
    Done,
    // ReSharper restore InconsistentNaming
}

/// <summary>What YouTube currently says about one video.</summary>
public sealed record VideoDetails(string Title, VideoAvailability Availability, DateOnly? PublishedOn, string? Reason);

/// <summary>A video the application has seen, with its processing state.</summary>
public sealed record TrackedVideo(
    long Id,
    long ChannelId,
    string YouTubeId,
    string Title,
    VideoState State,
    int Attempts,
    string? LastReason,
    DateTimeOffset FirstSeenAt);

/// <summary>A video that should get its first state in the store.</summary>
public sealed record NewVideo(long ChannelId, string YouTubeId, string Title, VideoState State, string? Reason, DateTimeOffset FirstSeenAt);