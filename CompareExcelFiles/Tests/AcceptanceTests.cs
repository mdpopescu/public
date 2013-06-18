using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void EndToEnd()
    {
      const string PATH = @"..\..\..\CompareExcelFiles\bin\Debug\CompareExcelFiles.exe";
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

      var startInfo = new ProcessStartInfo
      {
        CreateNoWindow = true,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        Arguments = ARGS,
        FileName = PATH,
      };
      var process = new Process { StartInfo = startInfo };
      process.Start();
      process.WaitForExit();

      var result = process.StandardOutput.ReadToEnd();

      Assert.AreEqual(EXPECTED_OUTPUT, result);
    }
  }
}