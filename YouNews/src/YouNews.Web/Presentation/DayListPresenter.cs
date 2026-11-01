using System.Globalization;
using YouNews.Core.Model;

namespace YouNews.Web.Presentation;

public sealed class DayListPresenter : IDayListPresenter
{
    public const int MAXIMUM_DAYS = 60;

    public IReadOnlyList<DayLinkView> Present(IReadOnlyList<DayCount> days, DateOnly? current)
    {
        var newestYear = days.Count > 0 ? days[0].Day.Year : 0;
        return days.Take(MAXIMUM_DAYS).Select(day => new DayLinkView(
            $"/day/{day.Day:yyyy-MM-dd}",
            day.Day.ToString(day.Day.Year == newestYear ? "MMM d" : "MMM d, yyyy", CultureInfo.InvariantCulture),
            day.EntryCount == 1 ? "1 video" : $"{day.EntryCount} videos",
            day.Day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            day.Day == current)).ToList();
    }
}