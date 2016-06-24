using System;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Screen
  {
    public Screen()
    {
      Events.Subscribe(EventType.MapChanged, Display);
      Events.Subscribe(EventType.ScoreChanged, DisplayScore);
      Events.Subscribe(EventType.LivesChanged, DisplayLives);
    }

    //

    private readonly object padlock = new object();

    private void Display(object sender, EventArgs e)
    {
      var location = ((MapChangeEventArgs) e).Location;
      var ch = ((MapChangeEventArgs) e).Value;

      lock (padlock)
      {
        Console.SetCursorPosition(location.X + 15, location.Y + 5);
        Console.Write(ch);
      }
    }

    private void DisplayScore(object sender, EventArgs e)
    {
      var score = ((ScoreEventArgs) e).Value;

      lock (padlock)
      {
        Console.SetCursorPosition(53, 1);
        Console.Write(string.Format("Score: {0:00000}", score));
      }
    }

    private void DisplayLives(object sender, EventArgs e)
    {
      var lives = ((LivesEventArgs) e).Value;

      lock (padlock)
      {
        Console.SetCursorPosition(53, 2);
        Console.Write(string.Format("Lives:    {0:00}", lives));
      }
    }
  }
}