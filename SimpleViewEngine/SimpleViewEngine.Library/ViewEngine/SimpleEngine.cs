using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Helpers;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Library.ViewEngine
{
  public class SimpleEngine : VirtualPathProviderViewEngine
  {
    public SimpleEngine(Engine engine, Lexer lexer, Parser parser)
    {
      this.engine = engine;
      this.lexer = lexer;
      this.parser = parser;

      ViewLocationFormats = new[] {"~/Views/{1}/{0}.simple", "~/Views/Shared/{0}.simple"};
      PartialViewLocationFormats = new[] {"~/Views/{1}/{0}.simple", "~/Views/Shared/{0}.simple"};
    }

    //

    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    {
      var template = Load(controllerContext, partialPath);
      var nodes = Parse(template);

      return new SimpleView(engine, nodes);
    }

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
      // we're not going to use the masterPath in this version... maybe later
      var template = Load(controllerContext, viewPath);
      var nodes = Parse(template);

      return new SimpleView(engine, nodes);
    }

    //

    private readonly Engine engine;
    private readonly Lexer lexer;
    private readonly Parser parser;

    private static string Load(ControllerContext controllerContext, string partialPath)
    {
      var physicalpath = controllerContext.HttpContext.Server.MapPath(partialPath);
      return File.ReadAllText(physicalpath);
    }

    private IEnumerable<Node> Parse(string template)
    {
      var tokens = lexer
        .Tokenize(template)
        .ToTokenList();

      return parser.Parse(tokens);
    }
  }
}