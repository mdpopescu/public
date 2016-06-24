using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Game
  {
    private int tickCount;

    public void Initialize()
    {
      tickCount = 0;

      Events.Raise(EventType.Create, this);
      Events.Raise(EventType.InitializeObjects, this);
    }

    public void Next()
    {
      Events.Raise(EventType.Tick, this, new TickEventArgs(tickCount++));
    }
  }
}