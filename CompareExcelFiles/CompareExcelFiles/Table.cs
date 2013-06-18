using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Brownstone.CompareExcelFiles
{
  public class Table
  {
    public string[] Columns { get; private set; }
    public RowList Rows { get; private set; }

    public static Table Load(string fileName)
    {
      var records = ReadExcel(fileName).ToList();

      return new Table(records.First(), new RowList(records.Skip(1)));
    }

    public Table(string[] columns, RowList rows)
    {
      Columns = columns;
      Rows = rows;
    }

    public void SortBy(IEnumerable<string> sortColumns)
    {
      var indices = GetIndices(sortColumns).ToList();
      Rows.SortBy(indices);
    }

    public Table ExcludeRecords(Table other, IEnumerable<string> compareColumns)
    {
      var indices = GetIndices(compareColumns).ToList();

      return new Table(Columns, Rows.Exclude(other.Rows, indices));
    }

    public void Dump(IEnumerable<string> columns, Action<string> writeLine)
    {
      var indices = GetIndices(columns).ToList();
      
      var header = Columns.ExtractColumns(indices);
      writeLine(string.Format("      {0}", header));

      var lines = Rows.ExtractColumns(indices);
      var i = 1;
      foreach (var line in lines)
        writeLine(string.Format("{0:d5} {1}", i++, line));
    }

    //

    private static IEnumerable<string[]> ReadExcel(string fileName)
    {
      using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      using (var excel = new ExcelPackage(file))
      {
        var sheet = excel.Workbook.Worksheets[1];
        var lastRow = sheet.Dimension.End.Row;
        var lastCol = sheet.Dimension.End.Column;

        for (var row = 1; row <= lastRow; row++)
        {
          var record = new List<string>();

          for (var col = 1; col <= lastCol; col++)
            record.Add(sheet.Cells[row, col].GetValue<string>() ?? "");

          yield return record.ToArray();
        }
      }
    }

    private IEnumerable<int> GetIndices(IEnumerable<string> sortColumns)
    {
      return sortColumns.Select(GetIndex);
    }

    private int GetIndex(string column)
    {
      return Columns.GetIndex(column);
    }
  }
}