namespace Renfield.SimpleViewEngine.Library.AST
{
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