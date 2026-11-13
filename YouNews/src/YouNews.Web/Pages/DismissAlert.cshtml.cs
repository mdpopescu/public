using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YouNews.Core.Ports;

namespace YouNews.Web.Pages;

public sealed class DismissAlertModel : PageModel
{
    public DismissAlertModel(IRunAlertStore alerts)
    {
        this.alerts = alerts;
    }

    public IActionResult OnGet() => Redirect("/");

    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken)
    {
        await alerts.DismissAsync(cancellationToken);
        return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
    }

    //

    private readonly IRunAlertStore alerts;
}