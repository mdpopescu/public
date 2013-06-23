using System;
using System.Web.Mvc;
using Renfield.SafeRedir.Models;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Controllers
{
  public class HomeController : Controller
  {
    public HomeController(ShorteningService shorteningService)
    {
      this.shorteningService = shorteningService;
    }

    [HttpGet]
    public ActionResult Index()
    {
      var model = new RedirectInfo();

      return View(model);
    }

    [HttpPost]
    public ContentResult Index(FormCollection form)
    {
      var url = NormalizeUrl(form["URL"]);
      var safeUrl = NormalizeUrl(form["SafeURL"] ?? Constants.DEFAULT_SAFE_URL);
      int ttl;
      if (!int.TryParse(form["TTL"], out ttl))
        ttl = Constants.DEFAULT_TTL;

      var id = shorteningService.CreateRedirect(url, safeUrl, ttl);

      var redirectLink = Url.RouteUrl("Redirect", new { id }, Request.Url.Scheme);

      return Content(redirectLink);
    }

    [HttpGet]
    public ActionResult r(string id)
    {
      var redirectResult = shorteningService.GetUrl(id);
      if (redirectResult == null)
        return new HttpNotFoundResult("Unknown id " + id);

      return redirectResult;
    }

    //

    private readonly ShorteningService shorteningService;

    private static string NormalizeUrl(string url)
    {
      var uri = new UriBuilder(url);

      return uri.Uri.ToString();
    }
  }
}