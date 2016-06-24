using System;

namespace Renfield.Snake.EventArguments
{
  public class PointEventArgs : EventArgs
  {
    public Point Location { get; private set; }

    public PointEventArgs(Point point)
    {
      Location = point;
    }
  }
}