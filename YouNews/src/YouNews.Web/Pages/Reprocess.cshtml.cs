using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Refresh;

namespace YouNews.Web.Pages;

public sealed class ReprocessModel : PageModel
{
    public ReprocessModel(IReprocessTrigger trigger)
    {
        this.trigger = trigger;
    }

    public IActionResult OnGet() => Redirect("/");

    public async Task<IActionResult> OnPostAsync(long entryId, string? returnUrl, CancellationToken cancellationToken)
    {
        await trigger.RequestAsync(entryId, cancellationToken);
        return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }

    //

    private readonly IReprocessTrigger trigger;
}