using System.Linq;
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
      return View();
    }

    public ActionResult GetStocks()
    {
      var data = logic
        .GetStocks()
        .Select(StockModel.From);
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //

    private readonly Logic logic;
  }
}