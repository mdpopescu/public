using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Library.ViewEngine
{
  public class SimpleView : IView
  {
    public SimpleView(Engine engine, IEnumerable<Node> nodes)
    {
      this.engine = engine;
      this.nodes = nodes;
    }

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      var parsedcontents = engine.Run(nodes, viewContext.ViewData.Model);

      writer.Write(parsedcontents);
    }

    //

    private readonly Engine engine;
    private readonly IEnumerable<Node> nodes;
  }
}