using System;
using System.Linq;
using Renfield.CompareExcelFiles2.Library;

namespace Renfield.CompareExcelFiles
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      if (args.Length < 3)
      {
        Console.WriteLine("Syntax: CompareExcelFiles file1 file2 column [column...]");
        Console.WriteLine("        file1     first file to compare");
        Console.WriteLine("        file2     second file to compare");
        Console.WriteLine("        column    name of column(s) to sort / compare by");

        return;
      }

      var excel = new ExcelLoader();
      var table1 = excel.Load(args[0]);
      var table2 = excel.Load(args[1]);

      var columns = args.Skip(2).ToArray();
      var comparer = new TableComparer(columns);

      var diff1 = comparer.Compare(table1, table2);
      var diff2 = comparer.Compare(table2, table1);

      DumpResult(1, args[0], diff1, table1.RowCount, columns);
      DumpResult(2, args[1], diff2, table2.RowCount, columns);
    }

    //

    private static void DumpResult(int fileIndex, string fileName, Table diff, int totalRows, string[] columns)
    {
      Console.WriteLine("** {0}: {1} ** --- {2} distinct rows out of {3}", fileIndex, fileName, diff.RowCount, totalRows);
      diff.Dump(columns, Console.WriteLine);
      Console.WriteLine();
    }
  }
}