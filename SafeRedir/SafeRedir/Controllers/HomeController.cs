using System.Web.Mvc;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      var model = new RedirectInfo();

      return View(model);
    }
  }
}