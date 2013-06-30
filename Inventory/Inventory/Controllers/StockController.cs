using System.Linq;
using System.Web.Mvc;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Controllers
{
  public class StockController : Controller
  {
    public StockController(Logic logic)
    {
      this.logic = logic;
    }

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult GetStock()
    {
      var data = logic
        .GetStock()
        .Select(it => new
        {
          it.Name,
          Quantity = it.Quantity.Formatted(),
          PurchaseValue = it.PurchaseValue.Formatted(),
          SaleValue = it.SaleValue.Formatted(),
        })
        .ToList();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //

    private readonly Logic logic;
  }
}