using System;
using System.Collections.Generic;
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
      var data = logic.GetAcquisitions();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetAcquisitionItems(int id)
    {
      var data = logic.GetAcquisitionItems(id);
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult Create()
    {
      var model = new AcquisitionModel
      {
        Date = DateTime.Today.ToString(Constants.DATE_FORMAT),
        Items = new List<AcquisitionItemModel> { new AcquisitionItemModel() }
      };

      return View(model);
    }

    [HttpPost]
    public ActionResult Create(AcquisitionModel model)
    {
      logic.AddAcquisition(model);

      return RedirectToAction("Create");
    }

    //

    private readonly Logic logic;
  }
}