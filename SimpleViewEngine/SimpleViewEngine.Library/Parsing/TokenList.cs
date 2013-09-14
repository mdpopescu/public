using System.Collections.Generic;
using System.Linq;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class TokenList
  {
    public static TokenList Create(IEnumerable<Token> tokens)
    {
      return new TokenList(tokens);
    }

    public TokenList(IEnumerable<Token> tokens)
    {
      this.tokens = tokens.ToList();
      index = 0;
    }

    public Token GetNext()
    {
      return index >= tokens.Count
               ? Token.EOF
               : tokens[index++];
    }

    //

    private int index;
    private readonly List<Token> tokens;
  }
}