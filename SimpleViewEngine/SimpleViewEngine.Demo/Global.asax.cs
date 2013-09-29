using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Helpers;
using Renfield.SimpleViewEngine.Library.Parsing;
using Renfield.SimpleViewEngine.Library.ViewEngine;

namespace Renfield.SimpleViewEngine.Demo
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
        );
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);

      RegisterViewEngine();
    }

    //

    private void RegisterViewEngine()
    {
      var lexer = new SimpleLexer();
      var parser = new SimpleParser(ParsingRules.Create);
      var engine = new Engine(templateName => Parse(lexer, parser, Context.Server.MapPath(templateName)));

      ViewEngines.Engines.Add(new SimpleEngine(engine, lexer, parser));
    }

    private static IEnumerable<Node> Parse(Lexer lexer, Parser parser, string template)
    {
      var tokens = lexer
        .Tokenize(template)
        .ToTokenList();

      return parser.Parse(tokens);
    }
  }
}