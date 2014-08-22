using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Renfield.GUIDService.Models;

namespace Renfield.GUIDService.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet]
    public ActionResult Index()
    {
      ViewBag.Title = "Home Page";

      return View(new GuidListModel());
    }

    [HttpPost]
    public async Task<ActionResult> Index(GuidListModel model)
    {
      var sw = new Stopwatch();
      sw.Start();

      using (var web = new HttpClient { BaseAddress = new Uri("http://localhost:36190/") })
      {
        var result = await web.GetAsync("api/guid/" + model.Count);
        result.EnsureSuccessStatusCode();

        var list = await result.Content.ReadAsAsync<IEnumerable<string>>();
        model.Values = list.ToList().Take(10);
      }

      sw.Stop();

      model.Duration = sw.Elapsed;

      return View(model);
    }
  }
}