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
      return View();
    }

    [AllowAnonymous]
    public ActionResult Contact()
    {
      return View();
    }

    [AllowAnonymous]
    public ActionResult Unauthorized()
    {
      return View();
    }

    [AllowAnonymous]
    public ActionResult PageNotFound()
    {
      return View();
    }
  }
}