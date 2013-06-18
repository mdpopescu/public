using System;
using System.Collections.Generic;
using System.Linq;

namespace Brownstone.CompareExcelFiles
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      if (args.Length < 3)
      {
        Console.WriteLine("Syntax: CompareExcelFiles file1 file2 column...");
        Console.WriteLine("        file1     first file to compare");
        Console.WriteLine("        file2     second file to compare");
        Console.WriteLine("        column    name of column(s) to sort by (defaults to unsorted)");

        return;
      }

      var fileName1 = args[0];
      var fileName2 = args[1];
      var columns = args.Skip(2).ToArray();

      var excel1 = GetTable(fileName1, columns);
      var excel2 = GetTable(fileName2, columns);

      var diff1 = excel1.ExcludeRecords(excel2, columns);
      var diff2 = excel2.ExcludeRecords(excel1, columns);

      Console.WriteLine("** 1: {0} ** --- {1} distinct rows out of {2}", fileName1, diff1.Rows.Count, excel1.Rows.Count);
      diff1.Dump(columns, Console.WriteLine);
      Console.WriteLine();

      Console.WriteLine("** 2: {0} ** --- {1} distinct rows out of {2}", fileName2, diff2.Rows.Count, excel2.Rows.Count);
      diff2.Dump(columns, Console.WriteLine);
      Console.WriteLine();
    }

    private static Table GetTable(string fileName, IEnumerable<string> columns)
    {
      var table = Table.Load(fileName);
      table.SortBy(columns);

      return table;
    }
  }
}