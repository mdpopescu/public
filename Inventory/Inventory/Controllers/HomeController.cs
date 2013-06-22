using System.Web.Mvc;
using Renfield.Inventory.Models;

namespace Renfield.Inventory.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Message = "Welcome to Inventory!";

      return View();
    }

    public ActionResult NewPurchase()
    {
      var order = new NewPurchaseOrder();

      return View(order);
    }
  }
}