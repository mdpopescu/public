using System.Web.Http;

namespace Renfield.GUIDService.App_Start
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{count}",
          defaults: new { count = RouteParameter.Optional }
      );
    }
  }
}
