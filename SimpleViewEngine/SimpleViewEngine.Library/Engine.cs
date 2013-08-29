using System.Linq;

namespace Renfield.SimpleViewEngine.Library
{
  public class Engine
  {
    public Engine(Parser parser)
    {
      this.parser = parser;
    }

    public string Run(string template, dynamic model)
    {
      var nodes = parser.Parse(template);

      return string.Join("", nodes.Select(it => it.Eval(model)));
    }

    //

    private readonly Parser parser;
  }
}