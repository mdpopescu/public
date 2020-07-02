using DecoratorGen.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecoratorGen.Tests.Services
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

        [TestClass]
        public class ExtractInterface : ParserTests
        {
            [TestMethod]
            public void ExtractsTheFirstInterfaceFromTheGivenCodeFragment()
            {
                const string FRAGMENT = @"class A
{
    public int B { get; set; }
}

public interface IX
{
    public string C { get; set; }
}

internal class D
{
}

public interface IY
{}
";
                var result = sut.ExtractInterface(FRAGMENT);

                Assert.AreEqual("IX", result.Name);
                Assert.AreEqual(
                    @"public interface IX
{
    public string C { get; set; }
}",
                    result.Code);
            }
        }
    }
}