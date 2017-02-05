using System.IO;
using DINGO.Library.Implementations;
using DINGO.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DINGO.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void SingleFile()
        {
            const string PROJECT = "output=_Output\r\nfile=_TestCode\\WinFileSystem.cs";
            const string OUTPUT_FILE = "_Output\\WinFileSystem.html";

            var generator = new Generator();
            generator.Run(PROJECT);

            Assert.IsTrue(File.Exists(OUTPUT_FILE));
            Assert.AreEqual(Resources.HTML_WinFileSystem, File.ReadAllText(OUTPUT_FILE));
        }
    }
}