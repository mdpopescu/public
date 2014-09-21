using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebGame.Filters
{
  public class RoleFilterAttribute : ActionFilterAttribute
  {
    public string Roles { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var user = filterContext.HttpContext.User;
      if (user == null)
        filterContext.Result = new HttpUnauthorizedResult();
      else
      {
        var roles = Roles.Split(',');
        if (!roles.Any(user.IsInRole))
          filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Unauthorized" }));
      }

      base.OnActionExecuting(filterContext);
    }
  }
}