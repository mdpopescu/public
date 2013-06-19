using System;

namespace Renfield.CompareExcelFiles
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("Syntax: CompareExcelFiles file1 file2 column [column...]");
      Console.WriteLine("        file1     first file to compare");
      Console.WriteLine("        file2     second file to compare");
      Console.WriteLine("        column    name of column(s) to sort / compare by");
    }
  }
}