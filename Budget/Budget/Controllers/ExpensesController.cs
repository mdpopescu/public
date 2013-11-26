using Budget.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget.Controllers
{
  public class ExpensesController : Controller
  {
    public ExpensesController(Logic logic)
    {
      this.logic = logic;
    }

    public ActionResult Index()
    {
      var now = GlobalSettings.SystemTime();
      var recurring = logic.GetRecurringExpensesFor(now.Year, now.Month);
      var longTerm = logic.GetLongTermExpenses();

      return View();
    }

    //

    Logic logic;
  }
}