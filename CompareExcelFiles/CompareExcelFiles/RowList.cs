using System;
using System.Collections.Generic;
using System.Linq;

namespace Brownstone.CompareExcelFiles
{
  public class RowList
  {
    public int Count
    {
      get { return rows.Count(); }
    }

    public RowList(IEnumerable<string[]> rows)
    {
      this.rows = rows;
    }

    public void SortBy(IEnumerable<int> indices)
    {
      indices = indices.ToList();

      var first = indices.First();
      var rest = indices.Skip(1);

      rows = rest
        .Aggregate(rows.OrderBy(row => row[first]),
          (current, index) => current.ThenBy(row => row[index]))
        .ToList();
    }

    public RowList Exclude(RowList other, List<int> compareColumns)
    {
      var records = rows
        .Where(row => !IsFoundIn(row, other.rows, compareColumns))
        .ToList();

      return new RowList(records);
    }

    public IEnumerable<string> ExtractColumns(List<int> indices)
    {
      return rows.Select(row => row.ExtractColumns(indices));
    }

    //

    private IEnumerable<string[]> rows;

    private static bool IsFoundIn(IList<string> row, IEnumerable<string[]> otherRows, IEnumerable<int> columns)
    {
      // do NOT call .ToList(), this MUST be lazily evaluated
      var comparisons = otherRows.Select(otherRow => CompareRow(row, otherRow, columns));
      
      // the list is sorted by the given columns; use that to stop when either equality is found,
      // or the given row is greater than the other
      foreach (var comparison in comparisons)
      {
        if (comparison == 0) // success
          return true;
        if (comparison < 0) // the rest can no longer be equal (the list is sorted)
          return false;
      }

      // reached the end of the list without finding equality
      return false;
    }

    private static int CompareRow(IList<string> row, IList<string> otherRow, IEnumerable<int> indices)
    {
      return indices
        .Select(index => String.Compare(row[index], otherRow[index], StringComparison.InvariantCultureIgnoreCase))
        .Where(comparison => comparison != 0)
        .FirstOrDefault();
    }
  }
}