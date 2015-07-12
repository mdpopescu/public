using System;

namespace Spiral
{
  public class MatrixGenerator
  {
    public MatrixGenerator(int n)
    {
      this.n = n;
    }

    public int[,] Generate()
    {
      matrix = new int[n, n];

      // the starting location is in the (truncated) middle of the matrix
      var x = (n - 1) / 2;
      var y = (n - 1) / 2;

      // go up initially - this will be changed to right immediately
      dx = 0;
      dy = -1;
      for (var i = 1; i <= n * n; i++)
      {
        matrix[x, y] = i;

        UpdatePosition(ref x, ref y);
      }

      return matrix;
    }

    //

    private readonly int n;

    private int[,] matrix;
    private int dx, dy;

    private void UpdatePosition(ref int x, ref int y)
    {
      int newdx;
      int newdy;
      GetNewDirection(out newdx, out newdy);

      // if we can switch direction, do so
      if (CanMoveTo(x, y, newdx, newdy))
      {
        dx = newdx;
        dy = newdy;
      }

      x += dx;
      y += dy;
    }

    private void GetNewDirection(out int newdx, out int newdy)
    {
      if (dx == 1 && dy == 0)
      {
        // right -> down
        newdx = 0;
        newdy = 1;
      }
      else if (dx == 0 && dy == 1)
      {
        // down -> left
        newdx = -1;
        newdy = 0;
      }
      else if (dx == -1 && dy == 0)
      {
        // left -> up
        newdx = 0;
        newdy = -1;
      }
      else if (dx == 0 && dy == -1)
      {
        // up -> right
        newdx = 1;
        newdy = 0;
      }
      else
      {
        throw new Exception("Unknown direction.");
      }
    }

    private bool CanMoveTo(int x, int y, int newdx, int newdy)
    {
      var newx = x + newdx;
      var newy = y + newdy;

      if (newx < 0 || newx >= n || newy < 0 || newy >= n)
        return false;

      return matrix[newx, newy] == 0;
    }
  }
}