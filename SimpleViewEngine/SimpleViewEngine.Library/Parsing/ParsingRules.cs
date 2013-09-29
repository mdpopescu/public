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
        {"property", value => new PropertyNode(ExtractPart(value, 0))},
        {"if", value => new ConditionalNode(ExtractPart(value, 1), InternalParse("endif"))},
        {"else", value => new ElseNode()},
        {"foreach", value => new RepeaterNode(ExtractPart(value, 1), InternalParse("endfor"))},
        {"include", BuildIncludeNode}
      };
    }

    protected IEnumerable<Node> InternalParse(string until)
    {
      return Generators
        .While(() => tokens.GetNext(), token => token.Type != until)
        .Select(ReadNode)
        .ToList();
    }

    protected static string ExtractPart(string value, int index)
    {
      var parts = value
        .Substring(2, value.Length - 4)
        .Split(' ');

      return index < parts.Length ? parts[index] : "";
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

    private static IncludeNode BuildIncludeNode(string value)
    {
      return new IncludeNode(ExtractPart(value, 2), ExtractPart(value, 1));
    }
  }
}