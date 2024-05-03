using DiamondKata.Library.Services;
using NSubstitute;

namespace DiamondKata.Tests.Services;

[TestClass]
public class DiamondBuilderTests
{
    private readonly DiamondBuilder sut;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public DiamondBuilderTests()
    {
        sut = new DiamondBuilder();
    }

    [TestClass]
    public class Build : DiamondBuilderTests
    {
        private readonly TextWriter writer = Substitute.For<TextWriter>();

        private readonly List<string> result = new();

        public Build()
        {
            writer.When(it => it.WriteLine(Arg.Any<string>())).Do(ci => result.Add((string)ci[0]));
        }

        [DataRow("0")]
        [DataRow("!")]
        [DataRow("~")]
        [DataRow("\\u1F600")]
        [DataRow("ABC")]
        [TestMethod("Out of range or multiple characters do not write anything to the output")]
        public void NoOutput(string arg)
        {
            sut.Build([arg], writer);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod("Single line")]
        public void Test2()
        {
            sut.Build(["A"], writer);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A", result[0]);
        }

        [TestMethod("Three lines")]
        public void Test3()
        {
            sut.Build(["B"], writer);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(" A ", result[0]);
            Assert.AreEqual("B B", result[1]);
            Assert.AreEqual(" A ", result[2]);
        }

        [TestMethod("Five lines")]
        public void Test4()
        {
            sut.Build(["C"], writer);

            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("  A  ", result[0]);
            Assert.AreEqual(" B B ", result[1]);
            Assert.AreEqual("C   C", result[2]);
            Assert.AreEqual(" B B ", result[3]);
            Assert.AreEqual("  A  ", result[4]);
        }
    }
}