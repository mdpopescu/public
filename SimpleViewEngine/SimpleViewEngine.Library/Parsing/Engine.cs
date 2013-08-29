using System.Linq;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class Engine
  {
    public Engine(Lexer lexer, Parser parser)
    {
      this.lexer = lexer;
      this.parser = parser;
    }

    public string Run(string template, object model)
    {
      var tokens = lexer.Tokenize(template);
      var nodes = parser.Parse(tokens);

      return string.Join("", nodes.Select(it => it.Eval(model)));
    }

    //

    private readonly Lexer lexer;
    private readonly Parser parser;
  }
}