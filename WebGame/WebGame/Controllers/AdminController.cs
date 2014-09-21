using System.Web.Mvc;
using WebGame.Filters;

namespace WebGame.Controllers
{
  [RoleFilter(Roles = "Admin")]
  public class AdminController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}