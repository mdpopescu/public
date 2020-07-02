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
            public void GeneratesTheCorrectNameForFilesStartingWithTheLetterIFollowedByAnotherUppercaseLetter()
            {
                const string FILENAME = @"c:\temp\ISomething.cs";

                var result = sut.Generate(FILENAME);

                Assert.AreEqual(@"c:\temp\SomethingDecorator.cs", result);
            }

            [TestMethod]
            public void GeneratesTheCorrectNameForFilesNotStartingWithTheLetterI()
            {
                const string FILENAME = @"c:\temp\Something.cs";

                var result = sut.Generate(FILENAME);

                Assert.AreEqual(@"c:\temp\SomethingDecorator.cs", result);
            }

            [TestMethod]
            public void GeneratesTheCorrectNameForFilesStartingWithTheLetterIFollowedByALowercaseLetter()
            {
                const string FILENAME = @"c:\temp\Interesting.cs";

                var result = sut.Generate(FILENAME);

                Assert.AreEqual(@"c:\temp\InterestingDecorator.cs", result);
            }
        }
    }
}