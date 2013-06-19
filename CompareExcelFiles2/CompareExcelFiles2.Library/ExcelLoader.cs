using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Renfield.CompareExcelFiles2.Library
{
  public class ExcelLoader
  {
    public Table Load(string fileName)
    {
      return new MemoryTable(ReadExcel(fileName));
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
  }
}