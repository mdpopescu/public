using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
00001 1 4
00002 3 4

";

      var result = RunAndCaptureOutput(ARGS);

      Assert.AreEqual(EXPECTED_OUTPUT, result);
    }

    [TestMethod]
    public void ReturnsHelp()
    {
      const string ARGS = @"..\..\..\file1.xlsx ..\..\..\file2.xlsx C A";
      const string EXPECTED_OUTPUT = @"Syntax: CompareExcelFiles file1 file2 column [column...]
        file1     first file to compare
        file2     second file to compare
        column    name of column(s) to sort / compare by
";
      var result = RunAndCaptureOutput(ARGS);

      Assert.AreEqual(EXPECTED_OUTPUT, result);
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