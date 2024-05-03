using DiamondKata.Library.Services;

namespace DiamondKata.Tests.Services;

[TestClass]
public class DiamondBuilderTests
{
    private DiamondBuilder sut;

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public DiamondBuilderTests()
    {
        sut = new DiamondBuilder();
    }
}