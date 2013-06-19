using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.CompareExcelFiles2.Library;

namespace Renfield.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void EndToEnd()
    {
      const string ARGS = @"..\..\..\file1.xlsx ..\..\..\file2.xlsx C A";
      const string EXPECTED_OUTPUT = @"** 1: ..\..\..\file1.xlsx ** --- 3 distinct rows out of 9
      C A
00001 2 1
00002 2 2
00003 2 3

** 2: ..\..\..\file2.xlsx ** --- 2 distinct rows out of 8
      C A
00001 3 4
00002 1 4

";

      var result = RunAndCaptureOutput(ARGS);

      Assert.AreEqual(EXPECTED_OUTPUT, result);
    }

    [TestMethod]
    public void ReturnsHelp()
    {
      const string EXPECTED_OUTPUT = @"Syntax: CompareExcelFiles file1 file2 column [column...]
        file1     first file to compare
        file2     second file to compare
        column    name of column(s) to sort / compare by
";
      var result = RunAndCaptureOutput("");

      Assert.AreEqual(EXPECTED_OUTPUT, result);
    }

    [TestMethod]
    public void LoadsExcelFile()
    {
      const string FILE_NAME = @"..\..\..\file1.xlsx";

      var sut = new ExcelLoader();

      var result = sut.Load(FILE_NAME);

      Assert.AreEqual(9, result.RowCount); // number of *data* rows
      Assert.AreEqual(4, result.ColCount);
      CollectionAssert.AreEqual(new[] { "A", "B", "C", "D" }, result.Columns);
      CollectionAssert.AreEqual(new[] { "1", "10", "1", "100" }, result.Data[0]);
      CollectionAssert.AreEqual(new[] { "2", "20", "1", "200" }, result.Data[1]);
      CollectionAssert.AreEqual(new[] { "3", "30", "1", "300" }, result.Data[2]);
      CollectionAssert.AreEqual(new[] { "1", "40", "2", "400" }, result.Data[3]);
      CollectionAssert.AreEqual(new[] { "2", "50", "2", "500" }, result.Data[4]);
      CollectionAssert.AreEqual(new[] { "3", "60", "2", "600" }, result.Data[5]);
      CollectionAssert.AreEqual(new[] { "1", "70", "3", "700" }, result.Data[6]);
      CollectionAssert.AreEqual(new[] { "2", "80", "3", "800" }, result.Data[7]);
      CollectionAssert.AreEqual(new[] { "3", "90", "3", "900" }, result.Data[8]);
    }

    //

    private static string RunAndCaptureOutput(string args)
    {
      const string PATH = @"..\..\..\CompareExcelFiles\bin\Debug\CompareExcelFiles.exe";

      var startInfo = new ProcessStartInfo
      {
        CreateNoWindow = true,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        Arguments = args,
        FileName = PATH,
      };
      var process = new Process { StartInfo = startInfo };
      process.Start();
      process.WaitForExit();

      return process.StandardOutput.ReadToEnd();
    }
  }
}