using System;

namespace Renfield.VM
{
  public static class Program
  {
    private static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        ShowHelp();
        return;
      }

      //..
    }

    private static void ShowHelp()
    {
      Console.WriteLine("Usage: vm [file name]");
      Console.WriteLine("  where   [file name] = the name of the (binary) file containing the program to run.");
    }
  }
}