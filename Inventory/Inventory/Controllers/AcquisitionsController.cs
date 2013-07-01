using System.Linq;
using System.Web.Mvc;
using Renfield.Inventory.Models;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Controllers
{
  public class AcquisitionsController : Controller
  {
    public AcquisitionsController(Logic logic)
    {
      this.logic = logic;
    }

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult GetAcquisitions()
    {
      var data = logic
        .GetAcquisitions()
        .Select(AcquisitionModel.From)
        .ToList();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //

    private readonly Logic logic;
  }
}