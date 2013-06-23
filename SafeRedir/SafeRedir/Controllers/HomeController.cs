using System.Web.Mvc;

namespace Renfield.SafeRedir.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Safe (time-limited) URL redirector";

      return View();
    }
  }
}
