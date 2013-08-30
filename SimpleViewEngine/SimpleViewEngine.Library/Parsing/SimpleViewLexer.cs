namespace Renfield.SimpleViewEngine.Library.Parsing
{
  public class SimpleViewLexer : RegexLexer
  {
    public SimpleViewLexer()
    {
      AddDefinition(new TokenDefinition("if", @"\{\{if \w[\w|\d]*\}\}"));
      AddDefinition(new TokenDefinition("else", @"\{\{else\}\}"));
      AddDefinition(new TokenDefinition("endif", @"\{\{endif\}\}"));
      AddDefinition(new TokenDefinition("foreach", @"\{\{foreach \w[\w|\d]*\}\}"));
      AddDefinition(new TokenDefinition("endfor", @"\{\{endfor\}\}"));
      AddDefinition(new TokenDefinition("property", @"\{\{[\w|\d]*\}\}"));
      AddDefinition(new TokenDefinition("constant", "[^{]+"));
    }
  }
}