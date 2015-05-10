using System;
using System.IO;

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

      var bytes = LoadFile(args[0]);
      var vm = new Interpreter();
      vm.Run(bytes);
    }

    private static void ShowHelp()
    {
      Console.WriteLine("Usage: vm [file name]");
      Console.WriteLine("  where   [file name] = the name of the (binary) file containing the program to run.");
    }

    private static byte[] LoadFile(string fileName)
    {
      return File.ReadAllBytes(fileName);
    }
  }
}