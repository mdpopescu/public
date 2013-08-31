using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
      var list = (IEnumerable) GetProperty(model, name);

      var strings = list
        .Cast<object>()
        .SelectMany<object, string>(o => nodes.Select(it => it.Eval(o)));

      return string.Join("", strings);
    }

    //

    private readonly IEnumerable<Node> nodes;
  }
}