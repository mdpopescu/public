using System;
using System.Threading.Tasks;

namespace Propagators
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var result = MainAsync().Result;
      Console.WriteLine(result);
    }

    //

    private static async Task<object> MainAsync()
    {
      var x = new Cell();
      var g = new Cell();
      var h = new Cell();

      BuildNetwork(x, g, h);

      var result = await Compute(x, g, h, 2.0);
      return result;
    }

    private static void BuildNetwork(Cell input, Cell guess, Cell betterGuess)
    {
      var temp1 = new Cell();
      var temp2 = new Cell();
      var two = new Cell();

      var divider1 = new Propagator(arr => (double) arr[0] / (double) arr[1]);
      var adder = new Propagator(arr => (double) arr[0] + (double) arr[1]);
      var constant = new Propagator(arr => 2.0);
      var divider2 = new Propagator(arr => (double) arr[0] / (double) arr[1]);

      divider1.Connect(temp1, input, guess);
      adder.Connect(temp2, temp1, guess);
      constant.Connect(two);
      divider2.Connect(betterGuess, temp2, two);
    }

    private static async Task<object> Compute(Cell x, Cell g, Cell h, double value)
    {
      const double EPS = 1e-8;

      double oldGuess;
      var newGuess = value / 2.0;

      await x.SetValueAsync(value);
      do
      {
        oldGuess = newGuess;
        await g.SetValueAsync(oldGuess);
        newGuess = (double) h.Value;
      } while (Math.Abs(newGuess - oldGuess) > EPS);

      return newGuess;
    }
  }
}