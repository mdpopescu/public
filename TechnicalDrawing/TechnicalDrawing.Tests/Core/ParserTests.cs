using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnicalDrawing.Library.Core;

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
            Assert.AreEqual("L", result.Name);
            CollectionAssert.AreEqual(new[] { 1.1f, 2.2f, 3.3f }, result.Args);
        }
    }
}