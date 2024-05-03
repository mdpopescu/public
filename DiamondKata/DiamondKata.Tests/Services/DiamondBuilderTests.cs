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

        [TestMethod("Out of range or multiple characters do not write anything to the output")]
        [DataRow("0")]
        [DataRow("!")]
        [DataRow("~")]
        [DataRow("\\u1F600")]
        [DataRow("ABC")]
        public void Test1(string arg)
        {
            sut.Build([arg], writer);

            writer.DidNotReceive().WriteLine(Arg.Any<string>());
        }
    }
}