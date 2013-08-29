using System.Collections.Generic;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Demo.Models;

namespace Renfield.SimpleViewEngine.Demo.Controllers
{
  public class HomeController : Controller
  {
    //
    // GET: /Home/

    public ActionResult Index()
    {
      var model = new PageModel
      {
        Title = "sample page",
        People = new List<Person>
        {
          new Person {FirstName = "Gigi", LastName = "Meseriasu", HasPassport = true},
          new Person {FirstName = "Marcel", LastName = "Popescu", HasPassport = true},
          new Person {FirstName = "Radu", LastName = "Mazare", HasPassport = false},
        },
      };

      return View(model);
    }
  }
}