using System.Web.Mvc;

namespace Renfield.Inventory.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Welcome to Inventory!";

      return View();
    }

    public ActionResult About()
    {
      return View();
    }
  }
}
