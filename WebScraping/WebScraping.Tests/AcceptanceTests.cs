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

            var compiler = new MultiStepCompiler();
            var interpreter = new CSharpInterpreter();
            var sut = new Runner(compiler, interpreter);

            var sb = new StringBuilder();
            using (var env = ObjectMother.CreateEnvironment("", sb))
            {
                sut.Run(PROGRAM, env);
            }

            Assert.AreEqual("Hello, world\r\n", sb.ToString());
        }
    }
}