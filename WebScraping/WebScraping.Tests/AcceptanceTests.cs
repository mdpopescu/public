using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScraping.Library.Implementations;

namespace WebScraping.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void TestHelloWorld()
        {
            const string PROGRAM = "print 'Hello, world'";

            using (var input = new StringReader(""))
            {
                var sb = new StringBuilder();

                using (var output = new StringWriter(sb))
                {
                    var sut = new Runner(input, output);

                    sut.Run(PROGRAM);
                }

                Assert.AreEqual("Hello, world\r\n", sb.ToString());
            }
        }
    }
}