using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Ports;
using YouNews.Web.Presentation;

namespace YouNews.Web.Pages;

/// <summary>Shows the story cards of one day.</summary>
public abstract class DayPageModelBase(IEntryReader reader, IEntryPresenter presenter) : PageModel
{
    public string Heading { get; private set; } = "";

    public IReadOnlyList<EntryView> Entries { get; private set; } = [];

    public string? NotFoundMessage { get; private set; }

    //

    /// <summary>Shows the day, or the friendly "nothing here" page with a 404 status when it has no entries.</summary>
    protected async Task<IActionResult> ShowDayAsync(DateOnly day, CancellationToken cancellationToken)
    {
        var entries = await reader.ListForDayAsync(day, cancellationToken);
        if (entries.Count == 0)
            return NothingHere($"There are no entries for {day:yyyy-MM-dd}.");

        Heading = $"AI News for {day:yyyy-MM-dd}";
        Entries = entries.Select(presenter.Present).ToList();
        ViewData["Title"] = Heading;
        ViewData["CurrentDay"] = day;
        return Page();
    }

    protected IActionResult NothingHere(string message)
    {
        ViewData["Title"] = "Nothing here";
        NotFoundMessage = message;
        Response.StatusCode = StatusCodes.Status404NotFound;
        return Page();
    }
}