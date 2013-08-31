namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleLexer : RegexLexer
  {
    public SimpleLexer()
    {
      const string BEGIN = @"\{\{";
      const string END = @"\}\}";
      const string IDENT = @"\w[\w|\d]*";

      AddDefinition(new TokenDefinition("if", BEGIN + "if " + IDENT + END));
      AddDefinition(new TokenDefinition("else", BEGIN + "else" + END));
      AddDefinition(new TokenDefinition("endif", BEGIN + "endif" + END));
      AddDefinition(new TokenDefinition("foreach", BEGIN + "foreach " + IDENT + END));
      AddDefinition(new TokenDefinition("endfor", BEGIN + "endfor" + END));
      AddDefinition(new TokenDefinition("property", BEGIN + "(" + IDENT + ")?" + END));
      AddDefinition(new TokenDefinition("constant", @".+?(?=$|\{\{)"));
    }
  }
}