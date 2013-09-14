using System;
using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleParser : Parser
  {
    public SimpleParser(Func<TokenList, ParsingRules> parsingRulesFactory)
    {
      this.parsingRulesFactory = parsingRulesFactory;
    }

    public IEnumerable<Node> Parse(TokenList tokens)
    {
      return parsingRulesFactory
        .Invoke(tokens)
        .Parse();
    }

    //

    private readonly Func<TokenList, ParsingRules> parsingRulesFactory;
  }
}