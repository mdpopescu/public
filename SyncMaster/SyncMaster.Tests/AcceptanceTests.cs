using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyncMaster.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void SynchronizesTwoFolders()
    {
      var temp = Path.GetTempPath();

      var path1 = CreateFolder(temp);
      var path2 = CreateFolder(temp);

      // Create a file in each folder *before* the application starts
      File.WriteAllText(Path.Combine(path1, "s1.txt"), "abc");
      File.WriteAllText(Path.Combine(path2, "d1.txt"), "abc");

      // Launch the clients
      var exePath = GetExePath();
      var proc1 = Process.Start(exePath, "--port=51001 --peer=127.0.0.1:51002 --mode=sync --path=" + Quote(path1));
      var proc2 = Process.Start(exePath, "--port=51002 --peer=127.0.0.1:51001 --mode=sync --path=" + Quote(path2));

      // Create a file in each folder *after* the application starts
      File.WriteAllText(Path.Combine(path1, "s2.txt"), "def");
      File.WriteAllText(Path.Combine(path2, "d2.txt"), "def");

      Thread.Sleep(500);

      // Check the files in the first folder
      Assert.AreEqual("abc", File.ReadAllText(Path.Combine(path1, "d1.txt")));
      Assert.AreEqual("def", File.ReadAllText(Path.Combine(path1, "d2.txt")));

      // Check the files in the second folder
      Assert.AreEqual("abc", File.ReadAllText(Path.Combine(path2, "s1.txt")));
      Assert.AreEqual("def", File.ReadAllText(Path.Combine(path2, "s2.txt")));

      // Terminate the clients
      proc1.Kill();
      proc2.Kill();
    }

    //

    private static string CreateFolder(string temp)
    {
      var folder = Path.Combine(temp, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(folder);

      return folder;
    }

    private string GetExePath()
    {
      return @"..\..\..\SyncMaster\bin\Debug\SyncMaster.exe";
    }

    private static string Quote(string s)
    {
      const string Q = "\"";

      return Q + s.Replace(Q, Q + Q) + Q;
    }
  }
}