namespace Renfield.Snake.EventArguments
{
  public class MapChangeEventArgs : PointEventArgs
  {
    public char Value { get; private set; }

    public MapChangeEventArgs(Point point, char value)
      : base(point)
    {
      Value = value;
    }
  }
}