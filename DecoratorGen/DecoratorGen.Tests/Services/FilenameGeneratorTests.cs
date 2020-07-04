using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class FilenameGeneratorTests
    {
        private FilenameGenerator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new FilenameGenerator();
        }

        [TestClass]
        public class Generate : FilenameGeneratorTests
        {
            [TestMethod]
            public void GeneratesTheCorrectNameForAClass()
            {
                const string CODE = "class SomethingDecorator";

                var result = sut.Generate(CODE);

                Assert.AreEqual("SomethingDecorator.cs", result);
            }
        }
    }
}