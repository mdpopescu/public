using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScraping.Library.Implementations.StmtComp;

namespace WebScraping.Tests.Implementations.StmtComp
{
    [TestClass]
    public class PrintCompilerTests
    {
        private PrintCompiler sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new PrintCompiler();
        }

        [TestClass]
        public class CanHandle : PrintCompilerTests
        {
            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ThrowsForNullArgument()
            {
                sut.CanHandle(null);
            }

            [TestMethod]
            public void ReturnsFalseForEmptyArgument()
            {
                var result = sut.CanHandle(new string[0]);

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ReturnsTrueForPrintStatements()
            {
                var result = sut.CanHandle(new[] { "print 1" });

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void IsNotCaseSensitive()
            {
                var result = sut.CanHandle(new[] { "pRiNt 1" });

                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ReturnsFalseIfNotPrintStatement()
            {
                var result = sut.CanHandle(new[] { "printt 1" });

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ReturnsTrueForPrintWithoutArguments()
            {
                var result = sut.CanHandle(new[] { "print" });

                Assert.IsTrue(result);
            }
        }

        [TestClass]
        public class Compile : PrintCompilerTests
        {
            [TestMethod]
            public void CompilesPrintWithSingleNumericArgument()
            {
                var result = sut.Compile(new[] { "print 1" });

                Assert.AreEqual(1, result.Length);
                Assert.AreEqual("output.WriteLine(1);", result[0]);
            }

            [TestMethod]
            public void CompilesPrintWithStringArgumentInSingleQuotes()
            {
                var result = sut.Compile(new[] { "print 'a'" });

                Assert.AreEqual(1, result.Length);
                Assert.AreEqual("output.WriteLine(\"a\");", result[0]);
            }
        }
    }
}