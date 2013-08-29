using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Renfield.SimpleViewEngine.Library.Parsing;

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

      var engine = new Engine(CreateLexer(), new Parser());
      ViewEngines.Engines.Add(new Library.ViewEngine.SimpleViewEngine(engine));
    }

    //

    private static SimpleLexer CreateLexer()
    {
      var lexer = new SimpleLexer();
      lexer.AddDefinition(new TokenDefinition("if", @"\{\{if \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endif", @"\{\{endif\}\}"));
      lexer.AddDefinition(new TokenDefinition("foreach", @"\{\{foreach \w[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("endfor", @"\{\{endfor\}\}"));
      lexer.AddDefinition(new TokenDefinition("property", @"\{\{[\w|\d]*\}\}"));
      lexer.AddDefinition(new TokenDefinition("constant", "[^{]+"));

      return lexer;
    }
  }
}