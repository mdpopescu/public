namespace Renfield.SimpleViewEngine.Library.AST
{
  // from http://blogs.msdn.com/b/drew/archive/2009/12/31/a-simple-lexer-in-c-that-uses-regular-expressions.aspx

  public class TokenPosition
  {
    public int Index { get; private set; }
    public int Line { get; private set; }
    public int Column { get; private set; }

    public TokenPosition(int index, int line, int column)
    {
      Index = index;
      Line = line;
      Column = column;
    }
  }
}