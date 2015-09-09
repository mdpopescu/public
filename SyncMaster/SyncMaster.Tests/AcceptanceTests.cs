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

      // Launch the server
      var iisPath = GetIISPath();
      var serverPath = GetServerPath();
      var server = Process.Start(iisPath, string.Format("/path:{0} /port:51000", serverPath));

      // Launch the clients
      var clientPath = GetClientPath();
      var client1 = Process.Start(clientPath, "--id=C1 --port=51001 --server=\"http://localhost:51000/\" --mode=sync --path=" + Quote(path1));
      var client2 = Process.Start(clientPath, "--id=C2 --port=51002 --server=\"http://localhost:51000/\" --mode=sync --path=" + Quote(path2));

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
      client1.Kill();
      client2.Kill();

      // Terminate the server
      server.Kill();
    }

    //

    private static string CreateFolder(string temp)
    {
      var folder = Path.Combine(temp, Guid.NewGuid().ToString("N"));
      Directory.CreateDirectory(folder);

      return folder;
    }

    private static string GetIISPath()
    {
      var programFolder = Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles;
      return Path.Combine(Environment.GetFolderPath(programFolder), "IIS Express", "iisexpress.exe");
    }

    private static string GetServerPath()
    {
      return @"..\..\..\SyncMaster.Server\bin\Debug\SyncMaster.Server.exe";
    }

    private static string GetClientPath()
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