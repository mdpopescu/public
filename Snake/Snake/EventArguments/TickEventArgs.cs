using System;

namespace Renfield.Snake.EventArguments
{
  public class TickEventArgs : EventArgs
  {
    public int TickCount { get; private set; }

    public TickEventArgs(int tickCount)
    {
      TickCount = tickCount;
    }
  }
}