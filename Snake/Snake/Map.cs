using System;
using Renfield.Snake.EventArguments;

namespace Renfield.Snake
{
  public class Map
  {
    public Map()
    {
      Events.Subscribe(EventType.Create, (sender, e) => CreateMap());
      Events.Subscribe(EventType.ObstacleHit, (sender, e) => CreateMap());

      Events.Subscribe(EventType.Moving, (sender, e) => Display(e, Constants.EMPTY));
      Events.Subscribe(EventType.Moved, (sender, e) => Display(e, Constants.SNAKE));
      Events.Subscribe(EventType.NewBorn, (sender, e) => Display(e, Constants.RABBIT));
    }

    //

    private const int ROWS = 15;
    private const int COLS = 50;
    private readonly char[,] map = new char[ROWS,COLS];

    private void CreateMap()
    {
      for (var y = 0; y < ROWS; y++)
        for (var x = 0; x < COLS; x++)
          SetMap(x, y, IsEdge(x, y) ? Constants.EDGE : Constants.EMPTY);
    }

    private void Display(EventArgs e, char ch)
    {
      var location = ((PointEventArgs) e).Location;
      SetMap(location.X, location.Y, ch);
    }

    private void SetMap(int x, int y, char ch)
    {
      if (map[y, x] != Constants.EMPTY && ch != Constants.EMPTY && ch != Constants.EDGE)
        Events.Raise(EventType.CollisionDetected, this, new CollisionEventArgs(new Point(x, y), map[y, x], ch));

      map[y, x] = ch;
      Events.Raise(EventType.MapChanged, this, new MapChangeEventArgs(new Point(x, y), ch));
    }

    private static bool IsEdge(int x, int y)
    {
      return x == 0 || x == COLS - 1 || y == 0 || y == ROWS - 1;
    }
  }
}