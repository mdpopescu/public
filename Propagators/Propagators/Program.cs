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

      var temp1 = new Cell();
      var temp2 = new Cell();
      var two = new Cell();

      var divider1 = new Propagator(arr => (double) arr[0] / (double) arr[1]);
      var adder = new Propagator(arr => (double) arr[0] + (double) arr[1]);
      var constant = new Propagator(arr => 2.0);
      var divider2 = new Propagator(arr => (double) arr[0] / (double) arr[1]);

      divider1.Connect(new[] { x, g }, temp1);
      adder.Connect(new[] { temp1, g }, temp2);
      constant.Connect(null, two);
      divider2.Connect(new[] { temp2, two }, h);

      await x.SetValueAsync(2.0);
      await g.SetValueAsync(1.4);

      return h.Value;
    }
  }
}