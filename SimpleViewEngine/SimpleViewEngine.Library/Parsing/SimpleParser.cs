using System;
using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleParser : Parser
  {
    public SimpleParser(Func<IEnumerable<Token>, TokenStream> tokenStreamFactory)
    {
      this.tokenStreamFactory = tokenStreamFactory;
    }

    public IEnumerable<Node> Parse(IEnumerable<Token> tokens)
    {
      var tokenStream = tokenStreamFactory.Invoke(tokens);

      return tokenStream.Parse();
    }

    //

    private readonly Func<IEnumerable<Token>, TokenStream> tokenStreamFactory;
  }
}