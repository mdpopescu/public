using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests.Core
{
    [TestClass]
    public class ParserTests
    {
        private Parser sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Parser();
        }

        [TestMethod]
        public void ReturnsAParsedLine()
        {
            var result = sut.Parse("L 1.1 2.2 3.3");

            Assert.IsNotNull(result);
            Assert.AreEqual(CommandName.Line, result.Name);
            CollectionAssert.AreEqual(new[] { 1.1f, 2.2f, 3.3f }, result.Args);
        }

        [TestMethod]
        public void ReturnsNoneForEmptyLines()
        {
            var result = sut.Parse("  ");

            Assert.IsNotNull(result);
            Assert.AreEqual(CommandName.None, result.Name);
            Assert.IsNotNull(result.Args);
            Assert.AreEqual(0, result.Args.Length);
        }

        [TestMethod]
        public void IgnoresComments_1()
        {
            var result = sut.Parse(" # comment ");

            Assert.IsNotNull(result);
            Assert.AreEqual(CommandName.None, result.Name);
            Assert.IsNotNull(result.Args);
            Assert.AreEqual(0, result.Args.Length);
        }

        [TestMethod]
        public void IgnoresComments_2()
        {
            var result = sut.Parse("L 1.1 2.2 3.3 # comment");

            Assert.IsNotNull(result);
            Assert.AreEqual(CommandName.Line, result.Name);
            CollectionAssert.AreEqual(new[] { 1.1f, 2.2f, 3.3f }, result.Args);
        }
    }
}