using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Refresh;

namespace YouNews.Web.Pages;

/// <summary>Answers the progress poll of the page script.</summary>
public sealed class StatusModel : PageModel
{
    public StatusModel(IRefreshStatus status)
    {
        this.status = status;
    }

    public IActionResult OnGet()
    {
        var current = status.Current;
        Response.Headers.CacheControl = "no-store";
        return new JsonResult(new StatusResponse(current?.Activity ?? "", current?.VideosDone ?? 0, current?.VideosTotal ?? 0, current is not null));
    }

    //

    private readonly IRefreshStatus status;

    private sealed record StatusResponse(string Activity, int VideosDone, int VideosTotal, bool Running);
}