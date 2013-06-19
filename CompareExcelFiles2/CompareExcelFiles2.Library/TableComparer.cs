using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.CompareExcelFiles2.Library
{
  public class TableComparer
  {
    public TableComparer(string[] columns)
    {
      this.columns = columns;
    }

    public Table Compare(Table table1, Table table2)
    {
      var indices = table1.Columns.GetIndices(columns).ToList();

      var rows = table1
        .Data
        .Where(row => !RowFoundIn(row, table2.Data, indices));

      return new MemoryTable(Enumerable.Repeat(table1.Columns, 1).Concat(rows));
    }

    //

    private readonly string[] columns;

    private static bool RowFoundIn(IList<string> row, IEnumerable<string[]> data, IEnumerable<int> indices)
    {
      // can't use .FirstOrDefault() here, the default is 0
      var comparisons = data
        .Select(otherRow => CompareRows(row, otherRow, indices))
        .Where(comparison => comparison == 0)
        .Take(1)
        .ToList();

      return comparisons.Any();
    }

    private static int CompareRows(IList<string> row1, IList<string> row2, IEnumerable<int> indices)
    {
      return indices
        .Select(index => string.Compare(row1[index], row2[index], StringComparison.InvariantCultureIgnoreCase))
        .Where(comparison => comparison != 0)
        .FirstOrDefault();
    }
  }
}