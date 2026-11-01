using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

/// <summary>Builds the list of days on the left.</summary>
public interface IDayListPresenter
{
    /// <summary>Lists at most the most recent days, marking the current one.</summary>
    IReadOnlyList<DayLinkView> Present(IReadOnlyList<DayCount> days, DateOnly? current);
}