using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheGame.Models;

namespace TheGame.Controllers
{
  public class GameController : Controller
  {
    public ActionResult Create()
    {
      var guid = Guid.NewGuid().ToString();
      return RedirectToAction("Join", new { id = guid });
    }

    public ActionResult Join(string id)
    {
      var model = new Game(id);
      return View(model);
    }
  }
}