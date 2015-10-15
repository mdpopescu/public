using System;

namespace Propagators
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var machine = new SqrtMachine();

      var result = machine.Compute(2.0);
      Console.WriteLine(result);
    }
  }
}