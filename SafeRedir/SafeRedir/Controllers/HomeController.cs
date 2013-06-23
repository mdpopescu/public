﻿using System.Web.Mvc;
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
      var url = form["URL"];
      var safeUrl = form["SafeURL"] ?? Constants.DEFAULT_SAFE_URL;
      int ttl;
      if (!int.TryParse(form["TTL"], out ttl))
        ttl = Constants.DEFAULT_TTL;

      var id = shorteningService.Shorten(url, safeUrl, ttl);

      var redirectLink = Url.RouteUrl("Redirect", new { id });

      return Content(redirectLink);
    }

    //

    private readonly ShorteningService shorteningService;
  }
}