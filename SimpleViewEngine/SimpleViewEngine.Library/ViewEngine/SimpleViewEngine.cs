using System.IO;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Library.ViewEngine
{
  public class SimpleViewEngine : VirtualPathProviderViewEngine
  {
    public SimpleViewEngine(Engine engine)
    {
      this.engine = engine;

      ViewLocationFormats = new[] {"~/Views/{1}/{0}.simple", "~/Views/Shared/{0}.simple"};
      PartialViewLocationFormats = new[] {"~/Views/{1}/{0}.simple", "~/Views/Shared/{0}.simple"};
    }

    //

    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    {
      var contents = Load(controllerContext, partialPath);
      return new SimpleView(engine, contents);
    }

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
      // we're not going to use the masterPath in this version... maybe later
      var contents = Load(controllerContext, viewPath);
      return new SimpleView(engine, contents);
    }

    //

    private static string Load(ControllerContext controllerContext, string partialPath)
    {
      var physicalpath = controllerContext.HttpContext.Server.MapPath(partialPath);
      return File.ReadAllText(physicalpath);
    }

    //

    private readonly Engine engine;
  }
}