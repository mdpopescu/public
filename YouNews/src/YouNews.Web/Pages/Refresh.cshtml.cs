using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Refresh;

namespace YouNews.Web.Pages;

public sealed class RefreshModel : PageModel
{
    public RefreshModel(IRefreshTrigger trigger)
    {
        this.trigger = trigger;
    }

    public IActionResult OnGet() => Redirect("/");

    public IActionResult OnPost(string? returnUrl)
    {
        trigger.TryStart();
        return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }

    //

    private readonly IRefreshTrigger trigger;
}