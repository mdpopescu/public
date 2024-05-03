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

        [DataRow("0")]
        [DataRow("!")]
        [DataRow("~")]
        [DataRow("\\u1F600")]
        [DataRow("ABC")]
        [TestMethod("Out of range or multiple characters do not write anything to the output")]
        public void NoOutput(string arg)
        {
            sut.Build([arg], writer);

            writer.DidNotReceive().WriteLine(Arg.Any<string>());
        }

        [TestMethod("Single line")]
        public void Test2()
        {
            sut.Build(["A"], writer);

            writer.Received().WriteLine("A");

            // I am tempted to replace the several tests with a complex LINQ expression in the refactoring step
            // after the second test passes but I think it might be harder to read.
        }

        [TestMethod("Two lines")]
        public void Test3()
        {
            sut.Build(["B"], writer);

            writer.Received().WriteLine(" A ");
            writer.Received().WriteLine("B B");
            writer.Received().WriteLine(" A ");
        }

        [TestMethod("Three lines")]
        public void Test4()
        {
            sut.Build(["C"], writer);

            writer.Received().WriteLine("  A  ");
            writer.Received().WriteLine(" B B ");
            writer.Received().WriteLine("C   C");
            writer.Received().WriteLine(" B B ");
            writer.Received().WriteLine("  A  ");
        }
    }
}