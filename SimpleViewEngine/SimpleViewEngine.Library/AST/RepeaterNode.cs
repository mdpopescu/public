using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class RepeaterNode : PropertyNode
  {
    public RepeaterNode(string name, IEnumerable<Node> nodes)
      : base(name)
    {
      this.nodes = nodes;
    }

    public override string Eval(object model)
    {
      var sb = new StringBuilder();

      var list = (IEnumerable) GetProperty(model, name);
      foreach (var o in list)
      {
        var copy = o;
        sb.Append(string.Join("", nodes.Select(it => it.Eval(copy))));
      }

      return sb.ToString();
    }

    //

    private readonly IEnumerable<Node> nodes;
  }
}