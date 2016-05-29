using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScraping.Library.Implementations;

namespace WebScraping.Tests.Implementations
{
    [TestClass]
    public class MultiStepCompilerTests
    {
        private MultiStepCompiler sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MultiStepCompiler();
        }

        [TestMethod]
        public void ReturnsNullIfProgramIsEmpty()
        {
            var result = sut.Compile("  ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertsThePrintStatementToConsoleWriteLine()
        {
            const string EXPECTED = @"using System.IO;
public class Program
{
public static void Main(TextReader input, TextWriter output)
{
//1//
output.WriteLine(1);
}
}
";

            var result = sut.Compile("print 1");

            Assert.AreEqual(EXPECTED, result);
        }

        [TestMethod]
        public void ConvertsPrintStatementsWithStringArgumentsInSingleQuotes()
        {
            const string EXPECTED = @"using System.IO;
public class Program
{
public static void Main(TextReader input, TextWriter output)
{
//1//
output.WriteLine(""abc"");
}
}
";

            var result = sut.Compile("print 'abc'");

            Assert.AreEqual(EXPECTED, result);
        }
    }
}