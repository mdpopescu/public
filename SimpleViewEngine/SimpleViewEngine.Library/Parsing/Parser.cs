using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class Parser
  {
    public IEnumerable<Node> Parse(IEnumerable<Token> tokens)
    {
      var index = 0;

      return InternalParse(tokens.ToList(), ref index, Lexer.EOF);
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

        switch (token.Type)
        {
          case "constant":
            nodes.Add(new ConstantNode(token.Value));
            break;

          case "property":
            nodes.Add(new PropertyNode(token.Value.Substring(2, token.Value.Length - 4)));
            break;

          case "if":
            nodes.Add(new ConditionalNode(token.Value.Substring(5, token.Value.Length - 7),
              InternalParse(tokens, ref index, "endif")));
            break;

          default:
            throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", token.Type, token.Value));
        }
      }

      return nodes;
    }
  }
}