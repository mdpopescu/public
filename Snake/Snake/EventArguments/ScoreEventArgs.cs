using System;

namespace Renfield.Snake.EventArguments
{
  public class ScoreEventArgs : EventArgs
  {
    public int Value { get; private set; }

    public ScoreEventArgs(int value)
    {
      Value = value;
    }
  }
}