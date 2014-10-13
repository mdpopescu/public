using System;
using System.Diagnostics;
using System.Reflection;

namespace MaxRecursion
{
  internal class Program
  {
    private static readonly string appPath = Assembly.GetExecutingAssembly().Location;

    private static void Main(string[] args)
    {
      if (args.Length > 0)
      {
        Console.WriteLine("Trying with " + args[0]);
        RecursiveMethod(int.Parse(args[0]));
      }
      else
      {
        var result = BinarySearch(1, int.MaxValue, RecursiveMethod);
        Console.WriteLine(result);
      }
    }

    private static int BinarySearch(int min, int max, Action<int> action)
    {
      var l = min;
      var r = max;

      while (l <= r)
      {
        var m = l / 2 + r / 2;
        if (TryFunc(m))
          l = m + 1;
        else
          r = m - 1;
      }

      return l;
    }

    private static bool TryFunc(int n)
    {
      var process = Process.Start(appPath, n.ToString());
      process.WaitForExit();
      return process.ExitCode == 0;
    }

    private static void RecursiveMethod(int n)
    {
      if (n <= 0)
        return;

      RecursiveMethod(n - 1);
    }
  }
}