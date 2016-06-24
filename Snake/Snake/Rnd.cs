using System;

namespace Renfield.Snake
{
  public static class Rnd
  {
    private static readonly Random rng = new Random();

    public static int Get()
    {
      return rng.Next();
    }

    public static int Get(int minValue, int maxValue)
    {
      return rng.Next(minValue, maxValue);
    }

    public static Point GetPoint()
    {
      var x = Get(1, 49);
      var y = Get(1, 14);

      return new Point(x, y);
    }
  }
}