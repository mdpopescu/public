using System.Text.RegularExpressions;

namespace Renfield.SimpleViewEngine.Library.Parsing
{
  // from http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx

  public class TokenDefinition
  {
    public string Type { get; private set; }
    public Regex Regex { get; private set; }
    public bool IsIgnored { get; private set; }

    public TokenDefinition(string type, string pattern, bool isIgnored = false)
    {
      Type = type;
      Regex = new Regex(pattern, RegexOptions.Compiled |
                                 RegexOptions.CultureInvariant |
                                 RegexOptions.IgnoreCase |
                                 RegexOptions.Singleline);
      IsIgnored = isIgnored;
    }
  }
}