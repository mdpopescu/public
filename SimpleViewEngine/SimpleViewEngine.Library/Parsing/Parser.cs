using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public interface Parser
  {
    IEnumerable<Node> Parse(TokenList tokens);
  }
}