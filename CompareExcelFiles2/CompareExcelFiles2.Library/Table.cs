using System;

namespace Renfield.CompareExcelFiles2.Library
{
  public interface Table
  {
    int RowCount { get; }
    int ColCount { get; }

    string[] Columns { get; }
    string[][] Data { get; }

    void Dump(string[] columns, Action<string> writeLine);
  }
}