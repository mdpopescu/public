using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WClone.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void DownloadsASingleFileSite()
        {
            const string FILENAME = "index.html";
            const string CONTENTS = "<html><body>This is the default file.</body></html>";

            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "";
            var outputPath = Path.Combine(basePath, @"_output");

            DeleteFilesInFolder(outputPath);

            var port = Sys.FindAvailablePort();
            var web = new WebServer(port);
            web.Add($"/{FILENAME}", CONTENTS);

            using (web.Start())
            {
                var path = Path.Combine(basePath, @"..\..\..\WClone\bin\Debug\WClone.exe");
                var args = $@"-o {outputPath} http://localhost:{port}/{FILENAME}";

                Sys
                    .Run(path, args)
                    .WaitForExit();
            }

            var files = Directory.GetFiles(outputPath);
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(FILENAME, files[0]);
            Assert.AreEqual(CONTENTS, File.ReadAllText(files[0]));
        }

        private static void DeleteFilesInFolder(string path)
        {
            Directory.CreateDirectory(path);
            foreach (var filename in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(filename);
                }
                catch (IOException)
                {
                    //
                }
            }
        }
    }
}