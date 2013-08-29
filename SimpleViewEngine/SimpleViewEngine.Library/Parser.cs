using System;
using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library
{
  public class Parser
  {
    public IEnumerable<Node> Parse(string input)
    {
      while (!string.IsNullOrEmpty(input))
      {
        var index = input.IndexOf(Constants.HANDLE_OPEN, StringComparison.Ordinal);
        if (index < 0)
        {
          yield return new ConstantNode(input);
          break;
        }

        if (index > 0)
        {
          yield return new ConstantNode(input.Substring(0, index));
          input = input.Substring(index);
        }

        // now input starts with the {{ sequence; skip it
        input = input.Remove(0, Constants.HANDLE_OPEN.Length);
        index = input.IndexOf(Constants.HANDLE_CLOSE, StringComparison.Ordinal);
        yield return new PropertyNode(input.Substring(0, index));

        input = input.Substring(index + Constants.HANDLE_CLOSE.Length);
      }
    }
  }
}