using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;
using Renfield.SimpleViewEngine.Library.Helpers;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class ParsingRules
  {
    public static ParsingRules Create(TokenList tokens)
    {
      return new ParsingRules(tokens);
    }

    public ParsingRules(TokenList tokens)
    {
      rules = new Lazy<Dictionary<string, Func<string, Node>>>(CreateRules);
      this.tokens = tokens;
    }

    public IEnumerable<Node> Parse()
    {
      return InternalParse(Token.EOF.Type);
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

    protected IEnumerable<Node> InternalParse(string until)
    {
      return Generators
        .While(() => tokens.GetNext(), token => token.Type != until)
        .Select(ReadNode)
        .ToList();
    }

    protected static string ExtractName(string value, int start)
    {
      return value.Substring(start, value.Length - start - 2);
    }

    //

    private readonly Lazy<Dictionary<string, Func<string, Node>>> rules;
    private readonly TokenList tokens;

    private Node ReadNode(Token token)
    {
      Func<string, Node> handler;
      if (!rules.Value.TryGetValue(token.Type, out handler))
        throw new Exception(string.Format("Unexpected token of type [{0}] and value [{1}]", token.Type, token.Value));

      return handler(token.Value);
    }
  }
}