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
        public void ReturnsAnEmptyStringIfProgramIsEmpty()
        {
            var result = sut.Compile("  ");

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ConvertsThePrintStatementToConsoleWriteLine()
        {
            var result = sut.Compile("print 1");

            Assert.AreEqual("//1//\r\nConsole.WriteLine(1);\r\n", result);
        }

        [TestMethod]
        public void ConvertsPrintStatementsWithStringArgumentsInSingleQuotes()
        {
            var result = sut.Compile("print 'abc'");

            Assert.AreEqual("//1//\r\nConsole.WriteLine(\"abc\");\r\n", result);
        }
    }
}