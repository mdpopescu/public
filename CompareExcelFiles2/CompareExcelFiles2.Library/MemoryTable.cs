using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.CompareExcelFiles2.Library
{
  public class MemoryTable : Table
  {
    public int RowCount { get; private set; }
    public int ColCount { get; private set; }
    public string[] Columns { get; private set; }
    public string[][] Data { get; private set; }

    public MemoryTable(IEnumerable<string[]> cells)
    {
      cells = (cells ?? new List<string[]>()).ToList();

      RowCount = cells.Count() - 1;
      if (RowCount < 0)
        throw new Exception("At least one row is required.");

      Columns = cells.First();
      ColCount = Columns.Length;
      if (ColCount < 1)
        throw new Exception("At least one column is required.");

      Data = cells.Skip(1).ToArray();
    }
  }
}