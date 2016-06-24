namespace Renfield.Snake.EventArguments
{
  public class CollisionEventArgs : PointEventArgs
  {
    public char OldValue { get; private set; }
    public char NewValue { get; private set; }

    public CollisionEventArgs(Point point, char oldValue, char newValue)
      : base(point)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}