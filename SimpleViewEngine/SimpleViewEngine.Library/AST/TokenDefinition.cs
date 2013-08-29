using System.Text.RegularExpressions;

namespace Renfield.SimpleViewEngine.Library.AST
{
  public class TokenDefinition
  {
    public string Type { get; private set; }
    public Regex Regex { get; private set; }
    public bool IsIgnored { get; private set; }

    public TokenDefinition(string type, string pattern, bool isIgnored = false)
    {
      Type = type;
      Regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
      IsIgnored = isIgnored;
    }
  }
}