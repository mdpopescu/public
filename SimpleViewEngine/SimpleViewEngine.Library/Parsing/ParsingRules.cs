using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class ParsingRules
  {
    public static ParsingRules Create(IEnumerable<Token> tokens)
    {
      return new ParsingRules(tokens);
    }

    public ParsingRules(IEnumerable<Token> tokens)
    {
      rules = new Lazy<Dictionary<string, Func<string, Node>>>(CreateRules);
      this.tokens = tokens.ToList();
    }

    public IEnumerable<Node> Parse()
    {
      index = 0;

      return InternalParse(Token.EOF);
    }

    //

    protected virtual Dictionary<string, Func<string, Node>> CreateRules()
    {
      return new Dictionary<string, Func<string, Node>>
      {
        {"constant", value => new ConstantNode(value)},
        {"property", value => new PropertyNode(ExtractName(value, 2))},
        {"if", value => new ConditionalNode(ExtractName(value, 5), InternalParse("endif"))},
        {"else", value => new ElseNode()},
        {"foreach", value => new RepeaterNode(ExtractName(value, 10), InternalParse("endfor"))},
      };
    }

    protected IEnumerable<Node> InternalParse(string eof)
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

    protected static string ExtractName(string value, int start)
    {
      return value.Substring(start, value.Length - start - 2);
    }

    //

    private readonly Lazy<Dictionary<string, Func<string, Node>>> rules;
    private readonly List<Token> tokens;
    private int index;

    private Node ReadNode(string type, string value)
    {
      Func<string, Node> handler;
      if (!rules.Value.TryGetValue(type, out handler))
        throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", type, value));

      return handler(value);
    }
  }
}