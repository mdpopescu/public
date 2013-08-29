namespace Renfield.SimpleViewEngine.Library.Parsing
{
  // from http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx

  public class Token
  {
    public string Type { get; private set; }
    public string Value { get; private set; }
    public TokenPosition Position { get; private set; }

    public Token(string type, string value, TokenPosition position)
    {
      Type = type;
      Value = value;
      Position = position;
    }
  }
}