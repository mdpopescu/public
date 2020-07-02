using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecoratorGen.Tests.Services
{
    [TestClass]
    public class ClassNameGeneratorTests
    {
        private ClassNameGenerator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new ClassNameGenerator();
        }

        [TestClass]
        public class GenerateClassName : ClassNameGeneratorTests
        {
            [TestMethod]
            public void GeneratesTheCorrectNameForFilesStartingWithTheLetterIFollowedByAnotherUppercaseLetter()
            {
                const string CODE = "interface ISomething {}";

                var result = sut.GenerateClassName(CODE);

                Assert.AreEqual("SomethingDecorator", result);
            }

            [TestMethod]
            public void GeneratesTheCorrectNameForFilesNotStartingWithTheLetterI()
            {
                const string CODE = "interface Something {}";

                var result = sut.GenerateClassName(CODE);

                Assert.AreEqual("SomethingDecorator", result);
            }

            [TestMethod]
            public void GeneratesTheCorrectNameForFilesStartingWithTheLetterIFollowedByALowercaseLetter()
            {
                const string CODE = "interface Interesting {}";

                var result = sut.GenerateClassName(CODE);

                Assert.AreEqual("InterestingDecorator", result);
            }
        }
    }
}