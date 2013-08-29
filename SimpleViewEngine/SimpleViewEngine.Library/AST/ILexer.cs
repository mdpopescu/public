using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public interface ILexer
  {
    void AddDefinition(TokenDefinition tokenDefinition);
    IEnumerable<Token> Tokenize(string source);
  }
}