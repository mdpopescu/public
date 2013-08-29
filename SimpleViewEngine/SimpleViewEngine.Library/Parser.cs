using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library
{
  public class Parser
  {
    public IEnumerable<Node> Parse(string input)
    {
      var result = new List<Node>();

      if (!string.IsNullOrEmpty(input))
        result.Add(new ConstantNode(input));

      return result;
    }
  }
}