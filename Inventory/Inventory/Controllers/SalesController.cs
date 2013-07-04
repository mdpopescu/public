using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Renfield.Inventory.Models;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Controllers
{
  public class SalesController : Controller
  {
    public SalesController(Logic logic)
    {
      this.logic = logic;
    }

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult GetSales()
    {
      var data = logic.GetSales();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetSaleItems(int id)
    {
      var data = logic.GetSaleItems(id);
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult Create()
    {
      var model = new SaleModel
      {
        Date = DateTime.Today.ToString(Constants.DATE_FORMAT),
        Items = new List<SaleItemModel> { new SaleItemModel() }
      };

      return View(model);
    }

    [HttpPost]
    public ActionResult Create(SaleModel model)
    {
      logic.AddSale(model);

      return RedirectToAction("Create");
    }

    //

    private readonly Logic logic;
  }
}