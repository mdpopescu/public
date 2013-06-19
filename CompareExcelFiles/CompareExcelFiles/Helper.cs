using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.CompareExcelFiles
{
  public static class Helper
  {
    public static int GetIndex(this string[] columns, string column)
    {
      return Array.IndexOf(columns, column);
    }

    public static string Join(this IEnumerable<string> values, string separator)
    {
      return string.Join(separator, values);
    }

    public static string ExtractColumns(this IList<string> values, List<int> indices)
    {
      return indices
        .Select(index => values[index])
        .Join(" ");
    }
  }
}