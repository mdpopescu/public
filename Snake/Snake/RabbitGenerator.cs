using System;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class RabbitGenerator
  {
    public RabbitGenerator()
    {
      Events.Subscribe(EventType.Tick, OnTick);
    }

    //

    private void OnTick(object sender, EventArgs e)
    {
      if (Rnd.Get(0, 1000) < 4)
        Events.Raise(EventType.NewBorn, this, new PointEventArgs(Rnd.GetPoint()));
    }
  }
}