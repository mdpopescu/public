using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using YouNews.Core.Ports;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages;

public sealed class DayModel(IEntryReader reader, IEntryPresenter presenter) : DayPageModelBase(reader, presenter)
{
    public async Task<IActionResult> OnGetAsync(string date, CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day))
            return NothingHere("That is not a date YouNews knows how to show. Days look like 2026-11-01.");

        return await ShowDayAsync(day, cancellationToken);
    }
}