using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Describes how the last refresh went and what is still waiting.</summary>
public interface IStatusPresenter
{
    /// <summary>Builds the status from the last run, the videos that failed at least once or were abandoned, and the tool problems, and the pending queue.</summary>
    StatusView Present(RefreshRun? lastRun, IReadOnlyList<TrackedVideo> waiting, IReadOnlyList<string> toolProblems, PendingQueue queue);

    /// <summary>Builds what the banner at the top of every page shows: the tool problems and the alert not dismissed yet, if any.</summary>
    StatusView PresentBanner(IReadOnlyList<string> toolProblems, RunAlert? alert);
}

/// <summary>The pending videos in processing order, with the names of their channels.</summary>
public sealed record PendingQueue(IReadOnlyList<TrackedVideo> Videos, IReadOnlyDictionary<long, string> ChannelNames)
{
    public static readonly PendingQueue EMPTY = new([], new Dictionary<long, string>());
}