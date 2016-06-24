using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Snake
  {
    public Snake()
    {
      Events.Subscribe(EventType.InitializeObjects, OnInitialize);
      Events.Subscribe(EventType.Tick, OnTick);
      Events.Subscribe(EventType.ScoreChanged, OnScoreChanged);

      Events.Subscribe(EventType.LeftPressed, (sender, e) => SetDirection(-1, 0));
      Events.Subscribe(EventType.RightPressed, (sender, e) => SetDirection(1, 0));
      Events.Subscribe(EventType.UpPressed, (sender, e) => SetDirection(0, -1));
      Events.Subscribe(EventType.DownPressed, (sender, e) => SetDirection(0, 1));

      Events.Subscribe(EventType.CollisionDetected, OnCollision);
      Events.Subscribe(EventType.ObstacleHit, OnInitialize);
    }

    //

    private List<Point> body;
    private int speed;
    private int deltaX;
    private int deltaY;
    private bool rabbitEaten;

    private void OnInitialize(object sender, EventArgs e)
    {
      var head = Rnd.GetPoint();
      body = new List<Point> { head };
      speed = 10;

      Events.Raise(EventType.Moved, this, new PointEventArgs(body[0]));
    }

    private void OnTick(object sender, EventArgs e)
    {
      var tickCount = ((TickEventArgs) e).TickCount;
      if (tickCount % speed != 0)
        return;

      var head = body.First();
      var tail = body.Last();

      if (rabbitEaten)
        rabbitEaten = false;
      else
      {
        Events.Raise(EventType.Moving, this, new PointEventArgs(tail));
        body.Remove(tail);
      }

      head = new Point(head.X + deltaX, head.Y + deltaY);

      body.Insert(0, head);
      Events.Raise(EventType.Moved, this, new PointEventArgs(head));
    }

    private void OnScoreChanged(object sender, EventArgs e)
    {
      var score = ((ScoreEventArgs) e).Value;
      if (score % 100 == 0 && speed > 1)
        speed -= 1;
    }

// ReSharper disable ParameterHidesMember
    private void SetDirection(int deltaX, int deltaY)
// ReSharper restore ParameterHidesMember
    {
      this.deltaX = deltaX;
      this.deltaY = deltaY;
    }

    private void OnCollision(object sender, EventArgs e)
    {
      var collision = (CollisionEventArgs) e;
      if (collision.NewValue != Constants.SNAKE)
        return;

      if (collision.OldValue == Constants.RABBIT)
        rabbitEaten = true;
      else
        Events.Raise(EventType.ObstacleHit, this);
    }
  }
}