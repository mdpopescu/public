using System;
using System.Threading.Tasks;

namespace Propagators
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var x = new Cell();
      var g = new Cell();
      var h = new Cell();

      BuildNetwork(x, g, h);

      var result = Compute(x, g, h, 2.0);
      Console.WriteLine(result);
    }

    //

    private static void BuildNetwork(Cell input, Cell guess, Cell betterGuess)
    {
      var temp1 = new Cell();
      var temp2 = new Cell();
      var two = new Cell();

      var divider1 = new Propagator(arr => Task.FromResult((object) ((double) arr[0] / (double) arr[1])));
      var adder = new Propagator(arr => Task.FromResult((object) ((double) arr[0] + (double) arr[1])));
      var constant = new Propagator(arr => Task.FromResult((object) (2.0)));
      var divider2 = new Propagator(arr => Task.FromResult((object) ((double) arr[0] / (double) arr[1])));

      divider1.Connect(temp1, input, guess);
      adder.Connect(temp2, temp1, guess);
      constant.Connect(two);
      divider2.Connect(betterGuess, temp2, two);
    }

    private static object Compute(Cell x, Cell g, Cell h, double value)
    {
      const double EPS = 1e-8;

      double oldGuess;
      var newGuess = value / 2.0;

      x.SetValue(value);
      do
      {
        oldGuess = newGuess;
        g.SetValue(oldGuess);
        newGuess = (double) h.Value;
      } while (Math.Abs(newGuess - oldGuess) > EPS);

      return newGuess;
    }
  }
}