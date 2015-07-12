using System;

namespace Spiral
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      const int n = 6;

      var generator = new MatrixGenerator(n);
      var matrix = generator.Generate();
      for (var y = 0; y < n; y++)
      {
        for (var x = 0; x < n; x++)
          Console.Write("{0,4:D} ", matrix[x, y]);

        Console.WriteLine();
        Console.WriteLine();
      }
    }
  }
}