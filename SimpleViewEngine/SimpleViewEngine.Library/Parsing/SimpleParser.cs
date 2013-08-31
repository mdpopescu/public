using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleParser : Parser
  {
    public IEnumerable<Node> Parse(IEnumerable<Token> tokens)
    {
      var index = 0;

      return InternalParse(tokens.ToList(), ref index, Token.EOF);
    }

    //

    private static IEnumerable<Node> InternalParse(IReadOnlyList<Token> tokens, ref int index, string eof)
    {
      var nodes = new List<Node>();

      while (index < tokens.Count)
      {
        var token = tokens[index++];
        if (token.Type == eof)
          return nodes;

        var value = token.Value;
        switch (token.Type)
        {
          case "constant":
            nodes.Add(new ConstantNode(value));
            break;

          case "property":
            nodes.Add(new PropertyNode(ExtractName(value, 2)));
            break;

          case "if":
            nodes.Add(new ConditionalNode(ExtractName(value, 5), InternalParse(tokens, ref index, "endif")));
            break;

          case "else":
            nodes.Add(new ElseNode());
            break;

          case "foreach":
            nodes.Add(new RepeaterNode(ExtractName(value, 10), InternalParse(tokens, ref index, "endfor")));
            break;

          default:
            throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", token.Type, value));
        }
      }

      return nodes;
    }

    private static string ExtractName(string value, int start)
    {
      return value.Substring(start, value.Length - start - 2);
    }
  }
}