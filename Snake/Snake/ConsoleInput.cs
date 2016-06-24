using System;

namespace Renfield.Snake
{
  public class ConsoleInput
  {
    public ConsoleInput()
    {
      Events.Subscribe(EventType.Tick, OnTick);
    }

    //

    private void OnTick(object sender, EventArgs e)
    {
      if (!Console.KeyAvailable)
        return;

      var key = Console.ReadKey(true);
      Handle(key.Key);
    }

    private void Handle(ConsoleKey key)
    {
      switch (key)
      {
        case ConsoleKey.LeftArrow:
          Events.Raise(EventType.LeftPressed, this);
          break;

        case ConsoleKey.RightArrow:
          Events.Raise(EventType.RightPressed, this);
          break;

        case ConsoleKey.UpArrow:
          Events.Raise(EventType.UpPressed, this);
          break;

        case ConsoleKey.DownArrow:
          Events.Raise(EventType.DownPressed, this);
          break;

        default:
          break;
      }
    }
  }
}