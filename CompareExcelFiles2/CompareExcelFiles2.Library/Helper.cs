using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.CompareExcelFiles2.Library
{
  public static class Helper
  {
    public static IEnumerable<int> GetIndices(this string[] allColumns, IEnumerable<string> requiredColumns)
    {
      return requiredColumns.Select(col => Array.IndexOf(allColumns, col));
    }
  }
}