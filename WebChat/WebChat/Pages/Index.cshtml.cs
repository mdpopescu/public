using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebChat.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> logger = logger;

    public void OnGet()
    {
    }

    public IActionResult OnPostRegister([FromForm] string nickname)
    {
        HttpContext.Session.SetString("nickname", nickname);
        return RedirectToPage("Index");
    }
}