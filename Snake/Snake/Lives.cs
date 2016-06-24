using System;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Lives
  {
    public Lives()
    {
      Events.Subscribe(EventType.InitializeObjects, OnInitialize);
      Events.Subscribe(EventType.ObstacleHit, OnObstacleHit);
      Events.Subscribe(EventType.ScoreChanged, OnScoreChanged);
    }

    //

    private int lives;

    private void OnInitialize(object sender, EventArgs e)
    {
      lives = 5;
      Events.Raise(EventType.LivesChanged, this, new LivesEventArgs(lives));
    }

    private void OnObstacleHit(object sender, EventArgs e)
    {
      lives--;
      Events.Raise(EventType.LivesChanged, this, new LivesEventArgs(lives));

      if (lives <= 0)
        Events.Raise(EventType.GameLost, this);
    }

    private void OnScoreChanged(object sender, EventArgs e)
    {
      var score = ((ScoreEventArgs)e).Value;
      if (score == 0 || score % 500 != 0)
        return;

      lives++;
      Events.Raise(EventType.LivesChanged, this, new LivesEventArgs(lives));
    }
  }
}