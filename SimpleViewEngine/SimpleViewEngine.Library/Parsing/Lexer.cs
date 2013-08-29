using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  // from http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx

  public interface Lexer
  {
    void AddDefinition(TokenDefinition tokenDefinition);
    IEnumerable<Token> Tokenize(string source);
  }
}