using System.Collections.Generic;
using System.Linq;
using Renfield.SimpleViewEngine.Library.AST;

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
      var nodes = Parse(template);

      return string.Join("", Process(model, nodes));
    }

    //

    protected virtual IEnumerable<Node> Parse(string template)
    {
      var tokens = lexer.Tokenize(template);
      var nodes = parser.Parse(tokens);

      return nodes;
    }

    protected virtual IEnumerable<string> Process(object model, IEnumerable<Node> nodes)
    {
      return nodes.Select(it => it.Eval(model));
    }

    //

    private readonly Lexer lexer;
    private readonly Parser parser;
  }
}