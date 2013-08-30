using System.Collections.Generic;
using System.Linq;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class ConditionalNode : PropertyNode
  {
    public ConditionalNode(string name, IEnumerable<Node> nodes)
      : base(name)
    {
      nodesTrue = new List<Node>();
      nodesFalse = new List<Node>();

      var current = nodesTrue;
      foreach (var node in nodes)
      {
        if (node is ElseNode)
          current = nodesFalse;
        else
          current.Add(node);
      }
    }

    public override string Eval(object model)
    {
      var branch = (bool) GetProperty(model, name) ? nodesTrue : nodesFalse;
      return string.Join("", branch.Select(it => it.Eval(model)));
    }

    //

    private readonly List<Node> nodesTrue;
    private readonly List<Node> nodesFalse;
  }
}