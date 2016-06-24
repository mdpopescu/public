using System;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Score
  {
    public Score()
    {
      Events.Subscribe(EventType.InitializeObjects, (sender, e) => Events.Raise(EventType.ScoreChanged, this, new ScoreEventArgs(score)));
      Events.Subscribe(EventType.CollisionDetected, OnCollision);
    }

    //

    private int score;

    private void OnCollision(object sender, EventArgs e)
    {
      var collision = (CollisionEventArgs) e;
      if (collision.NewValue != Constants.SNAKE || collision.OldValue != Constants.RABBIT)
        return;

      score += 10;
      Events.Raise(EventType.ScoreChanged, this, new ScoreEventArgs(score));
    }
  }
}