using System;

namespace Renfield.Snake.EventArguments
{
  public class LivesEventArgs : EventArgs
  {
    public int Value { get; private set; }

    public LivesEventArgs(int value)
    {
      Value = value;
    }
  }
}