using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace httpd.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void ReturnsAnExistingFile()
    {
      var basePath = @"S:\GIT-public\httpd";
      var port = Sys.FindAvailablePort();

      var path = basePath + @"\httpd\bin\debug\httpd.exe";
      var args = $@"{port} {basePath}\httpd.Tests\Web";

      var process = Sys.Run(path, args);
      try
      {
        using (var web = new WebClient())
        {
          web.BaseAddress = "http://localhost:" + port;

          var response = web.DownloadString("one-byte.txt");
          Assert.AreEqual("z", response);
        }
      }
      finally
      {
        process.Kill();
      }
    }
  }
}