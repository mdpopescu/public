using System.Web.Mvc;
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
      var data = logic.GetAcquisitions();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetAcquisitionItems(int id)
    {
      var data = logic.GetAcquisitionItems(id);
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //

    private readonly Logic logic;
  }
}