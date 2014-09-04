using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Demo.Models.Home.Acquisitions;
using Renfield.SimpleViewEngine.Demo.Models.Home.Index;

namespace Renfield.SimpleViewEngine.Demo.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      var model = new IndexPageModel
      {
        Title = "sample page",
        Boss = new Person {FirstName = "Clark", LastName = "Kent", HasPassport = false},
        People = new List<Person>
        {
          new Person {FirstName = "Gigi", LastName = "Meseriasu", HasPassport = true},
          new Person {FirstName = "Marcel", LastName = "Popescu", HasPassport = true},
          new Person {FirstName = "Radu", LastName = "Mazare", HasPassport = false},
        },
        LinkToAcquisitions = Url.Action("Acquisitions"),
      };

      return View(model);
    }

    public ActionResult Acquisitions()
    {
      var items = new List<AcquisitionItem>
      {
        new AcquisitionItem
        {
          Id = 1,
          ProductName = "Hammer",
          Price = "3.99",
          Quantity = "25.00",
          Value = "99.75",
        },
        new AcquisitionItem
        {
          Id = 2,
          ProductName = "Nails x100",
          Price = "1.99",
          Quantity = "100.00",
          Value = "99.00",
        },
      };

      var acquisition = new Acquisition
      {
        Id = 1,
        CompanyName = "Microsoft",
        Date = DateTime.Today.ToShortDateString(),
        Items = items,
        Value = "198.75",
      };

      var model = new AcquisitionPageModel
      {
        ScriptUrl = Url.Content("~/Scripts/Home/Acquisitions/page.js"),
        LinkToCreate = Url.Action("Create", "Home"),
        Acquisition = acquisition,
      };

      return View(model);
    }
  }
}