using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class Engine
  {
    public string Run(IEnumerable<Node> nodes, object model)
    {
      return string.Join("", Process(nodes, model));
    }

    //

    private static IEnumerable<string> Process(IEnumerable<Node> nodes, object model)
    {
      return nodes.Select(it => it.Eval(model));
    }
  }
}