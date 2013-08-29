using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class ConditionalNode : PropertyNode
  {
    public ConditionalNode(string name, IEnumerable<Node> nodes)
      : base(name)
    {
      this.nodes = nodes;
    }

    public override string Eval(object model)
    {
      return string.Compare(base.Eval(model), "true", StringComparison.InvariantCultureIgnoreCase) == 0
               ? string.Join("", nodes.Select(it => it.Eval(model)))
               : "";
    }

    //

    private readonly IEnumerable<Node> nodes;
  }
}