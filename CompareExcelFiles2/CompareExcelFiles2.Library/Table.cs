namespace Renfield.CompareExcelFiles2.Library
{
  public interface Table
  {
    int RowCount { get; }
    int ColCount { get; }

    string[] Columns { get; }
    string[][] Data { get; }
  }
}