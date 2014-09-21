using System.Web.Mvc;

namespace WebGame.Controllers
{
  public class HomeController : Controller
  {
    [AllowAnonymous]
    public ActionResult Index()
    {
      return View();
    }

    [AllowAnonymous]
    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    [AllowAnonymous]
    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}