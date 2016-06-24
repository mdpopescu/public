using System;

namespace Renfield.Snake
{
  public class Point : IEquatable<Point>
  {
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
      X = x;
      Y = y;
    }

    #region Implementation of IEquatable<point>

    public bool Equals(Point other)
    {
      return other.X == X && other.Y == Y;
    }

    #endregion

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof (Point)) return false;
      return Equals((Point) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }

    public static bool operator ==(Point left, Point right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right)
    {
      return !left.Equals(right);
    }
  }
}