using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GWBasicCompiler.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void PrintsHelloWorld()
    {
      // compile test.bas to test.exe
      var process = Process.Start(@"..\..\..\GWBasicCompiler\bin\Debug\GWBasicCompiler.exe", "test");
      process.WaitForExit();

      // run test.exe and check the result
      process = CreateProcess("test.exe");
      var result = RunAndCaptureOutput(process);
      Assert.AreEqual("Hello World", result);
    }

    //

    private static Process CreateProcess(string fileName)
    {
      return new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = fileName,
          Arguments = "",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          CreateNoWindow = true
        }
      };
    }

    private static string RunAndCaptureOutput(Process process)
    {
      var sb = new StringBuilder();

      process.Start();
      while (!process.StandardOutput.EndOfStream)
      {
        sb.AppendLine(process.StandardOutput.ReadLine());
      }

      return sb.ToString();
    }
  }
}