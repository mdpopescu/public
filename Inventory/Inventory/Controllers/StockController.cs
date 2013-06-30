using System.Web.Mvc;

namespace Renfield.Inventory.Controllers
{
  public class StockController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}