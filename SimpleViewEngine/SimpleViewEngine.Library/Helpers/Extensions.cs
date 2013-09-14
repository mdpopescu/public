using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.Parsing;

namespace Renfield.SimpleViewEngine.Library.Helpers
{
  public static class Extensions
  {
    public static TokenList ToTokenList(this IEnumerable<Token> tokens)
    {
      return TokenList.Create(tokens);
    }
  }
}