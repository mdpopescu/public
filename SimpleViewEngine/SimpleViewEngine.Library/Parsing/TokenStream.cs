using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class TokenStream
  {
    public TokenStream(IEnumerable<Token> tokens)
    {
      this.tokens = tokens.ToList();
      index = 0;
    }

    public IEnumerable<Node> Parse()
    {
      return InternalParse(Token.EOF);
    }

    //

    private readonly List<Token> tokens;
    private int index;

    private IEnumerable<Node> InternalParse(string eof)
    {
      var nodes = new List<Node>();

      while (index < tokens.Count)
      {
        var token = tokens[index++];
        if (token.Type == eof)
          return nodes;

        var node = ReadNode(token.Type, token.Value);
        nodes.Add(node);
      }

      return nodes;
    }

    private Node ReadNode(string type, string value)
    {
      switch (type)
      {
        case "constant":
          return ReadConstantNode(value);

        case "property":
          return ReadPropertyNode(value);

        case "if":
          return ReadConditionalNode(value);

        case "else":
          return ReadElseNode();

        case "foreach":
          return ReadRepeaterNode(value);

        default:
          throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", type, value));
      }
    }

    private static ConstantNode ReadConstantNode(string value)
    {
      return new ConstantNode(value);
    }

    private static PropertyNode ReadPropertyNode(string value)
    {
      return new PropertyNode(ExtractName(value, 2));
    }

    private static ElseNode ReadElseNode()
    {
      return new ElseNode();
    }

    private ConditionalNode ReadConditionalNode(string value)
    {
      return new ConditionalNode(ExtractName(value, 5), InternalParse("endif"));
    }

    private RepeaterNode ReadRepeaterNode(string value)
    {
      return new RepeaterNode(ExtractName(value, 10), InternalParse("endfor"));
    }

    private static string ExtractName(string value, int start)
    {
      return value.Substring(start, value.Length - start - 2);
    }
  }
}