using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Describes how the last refresh went and what is still waiting.</summary>
public interface IStatusPresenter
{
    /// <summary>Builds the status from the last run, the videos that failed at least once or were abandoned, and the tool problems.</summary>
    StatusView Present(RefreshRun? lastRun, IReadOnlyList<TrackedVideo> waiting, IReadOnlyList<string> toolProblems);
}