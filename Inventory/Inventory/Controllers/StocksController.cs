using System.Web.Mvc;
using Renfield.Inventory.Models;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Controllers
{
  public class StocksController : Controller
  {
    public StocksController(Logic logic)
    {
      this.logic = logic;
    }

    public ActionResult Index()
    {
      return View(new StockModel());
    }

    public ActionResult GetStocks()
    {
      var data = logic.GetStocks();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //

    private readonly Logic logic;
  }
}