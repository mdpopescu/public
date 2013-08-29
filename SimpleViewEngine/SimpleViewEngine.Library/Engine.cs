using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

namespace Renfield.SimpleViewEngine.Library
{
  public class Engine
  {
    public Engine(ILexer lexer, Parser parser)
    {
      this.lexer = lexer;
      this.parser = parser;
    }

    public string Run(string template, dynamic model)
    {
      var tokens = lexer.Tokenize(template);
      var nodes = parser.Parse(tokens);

      return string.Join("", nodes.Select(it => it.Eval(model)));
    }

    //

    private readonly ILexer lexer;
    private readonly Parser parser;
  }
}