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
      if (!cells.Any())
        throw new Exception("At least one row is required.");

      RowCount = cells.Count() - 1;
    }
  }
}