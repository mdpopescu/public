namespace Renfield.SimpleViewEngine.Library.AST
{
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