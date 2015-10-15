using System;

namespace Propagators
{
  public class SqrtMachine
  {
    public SqrtMachine()
    {
      x = new Cell();
      g = new Cell();
      h = new Cell();

      var temp1 = new Cell();
      var temp2 = new Cell();
      var two = new Cell();

      var divider1 = new Propagator(arr => (double) arr[0] / (double) arr[1]);
      var adder = new Propagator(arr => (double) arr[0] + (double) arr[1]);
      var constant = new Propagator(arr => 2.0);
      var divider2 = new Propagator(arr => (double) arr[0] / (double) arr[1]);

      divider1.Connect(temp1, x, g);
      adder.Connect(temp2, temp1, g);
      constant.Connect(two);
      divider2.Connect(h, temp2, two);
    }

    public double Compute(double value)
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

    //

    private readonly Cell x;
    private readonly Cell g;
    private readonly Cell h;
  }
}