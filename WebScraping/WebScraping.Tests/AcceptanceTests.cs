using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScraping.Library.Implementations;
using WebScraping.Library.Implementations.StmtComp;

namespace WebScraping.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void TestHelloWorld()
        {
            const string PROGRAM = "print 'Hello, world'";

            var compiler = new MultiStepCompiler(new PrintCompiler());
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