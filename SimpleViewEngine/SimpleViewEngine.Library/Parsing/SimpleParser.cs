using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleParser : Parser
  {
    public IEnumerable<Node> Parse(IEnumerable<Token> tokens)
    {
      var tokenStream = new TokenStream(tokens);

      return tokenStream.Parse();
    }
  }
}