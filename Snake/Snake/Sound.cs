using System;

namespace Renfield.Snake
{
  public class Sound
  {
    public Sound()
    {
      Events.Subscribe(EventType.ScoreChanged, OnScoreChanged);
      Events.Subscribe(EventType.LivesChanged, OnLivesChanged);
      Events.Subscribe(EventType.GameLost, OnGameLost);
    }

    //

    private static void OnScoreChanged(object sender, EventArgs e)
    {
      Console.Beep(800, 100);
    }

    private static void OnLivesChanged(object sender, EventArgs e)
    {
      Console.Beep(600, 250);
    }

    private static void OnGameLost(object sender, EventArgs e)
    {
      Console.Beep(400, 500);
    }
  }
}