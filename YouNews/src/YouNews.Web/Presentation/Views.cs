namespace YouNews.Web.Presentation;

public sealed record TagView(string Text, string CssClass);

public sealed record SourceView(string Title, string Url);

public sealed record RelatedView(string Url, string Text);

/// <summary>A story card, ready to render without any logic.</summary>
public sealed record EntryView(
    long Id,
    string AnchorId,
    string PageUrl,
    string Headline,
    string ChannelName,
    string VideoUrl,
    IReadOnlyList<TagView> Tags,
    IReadOnlyList<string> SummaryParagraphs,
    IReadOnlyList<string> OpinionParagraphs,
    IReadOnlyList<SourceView> Sources,
    string? MainImageUrl,
    IReadOnlyList<string> FrameUrls,
    IReadOnlyList<RelatedView> Related,
    bool IsOffTopic,
    string ArtClass,
    bool IsReprocessQueued,
    string ReprocessLabel,
    string? ReprocessFailureText);

/// <summary>One day in the list on the left.</summary>
public sealed record DayLinkView(string Url, string Label, string CountText, string FullDate, bool IsCurrent);

/// <summary>Either the Refresh button (not running) or a progress bar.</summary>
public sealed record RefreshView(bool IsRunning, string Activity, int Done, int Total, int Percent, string Discovery = "");

public sealed record WaitingVideoView(string Title, string Url, string Reason, string Detail);

/// <summary>A video next in line to be processed; Detail holds the channel and publish date when known.</summary>
public sealed record QueuedVideoView(string Title, string Url, string Detail);

public sealed record StatusView(
    string? LastRunText,
    string? LastRunProblem,
    IReadOnlyList<WaitingVideoView> Waiting,
    IReadOnlyList<string> ToolProblems,
    string? QueueText,
    IReadOnlyList<QueuedVideoView> Queue,
    RunAlertView? Alert = null);

/// <summary>An alert about a run that stays on every page until dismissed; RaisedText says when it was raised.</summary>
public sealed record RunAlertView(string Message, string RaisedText);

public sealed record SetupView(bool HasData, string Hardware, IReadOnlyList<string> Models, string CheckedAtText, string? Warning, string CurrentModel, IReadOnlyList<string> ModelChoices);

/// <summary>The sidebar content of every page.</summary>
public sealed record SidebarView(IReadOnlyList<DayLinkView> Days, RefreshView Refresh, bool ChannelsCurrent);