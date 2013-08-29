using System.IO;
using System.Web.Mvc;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Library.ViewEngine
{
  public class SimpleView : IView
  {
    public SimpleView(Engine engine, string contents)
    {
      this.engine = engine;
      this.contents = contents;
    }

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      var parsedcontents = engine.Run(contents, viewContext.ViewData.Model);

      writer.Write(parsedcontents);
    }

    //

    private readonly Engine engine;
    private readonly string contents;
  }
}