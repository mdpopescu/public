namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleLexer : RegexLexer
  {
    public SimpleLexer()
    {
      const string BEGIN = @"\{\{";
      const string END = @"\}\}";
      const string IDENT = @"\w[\w|\d]*";
      const string PROPNAME = IDENT + @"(\." + IDENT + @")*";

      AddDefinition(new TokenDefinition("if", BEGIN + "if " + PROPNAME + END));
      AddDefinition(new TokenDefinition("else", BEGIN + "else" + END));
      AddDefinition(new TokenDefinition("endif", BEGIN + "endif" + END));
      AddDefinition(new TokenDefinition("foreach", BEGIN + "foreach " + PROPNAME + END));
      AddDefinition(new TokenDefinition("endfor", BEGIN + "endfor" + END));
      AddDefinition(new TokenDefinition("property", BEGIN + "(" + PROPNAME + ")?" + END));
      AddDefinition(new TokenDefinition("constant", @".+?(?=$|\{\{)"));
    }
  }
}