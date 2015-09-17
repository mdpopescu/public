using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class CompositeDetailsReaderTests
  {
    private Mock<DetailsReader> r1;
    private Mock<DetailsReader> r2;
    private CompositeDetailsReader sut;

    private Details d1;
    private Details d2;

    [TestInitialize]
    public void SetUp()
    {
      r1 = new Mock<DetailsReader>();
      r2 = new Mock<DetailsReader>();
      sut = new CompositeDetailsReader(r1.Object, r2.Object);

      d1 = new Details {Company = "c1", Product = "p1"};
      d2 = new Details {Company = "c2", Product = "p2"};

      r1.Setup(it => it.Read()).Returns(d1);
      r2.Setup(it => it.Read()).Returns(d2);
    }

    [TestMethod]
    public void ReturnsTheCompanyFromTheFirstReader()
    {
      var result = sut.Read();

      Assert.AreEqual("c1", result.Company);
    }

    [TestMethod]
    public void ReturnsTheProductFromTheFirstReader()
    {
      var result = sut.Read();

      Assert.AreEqual("p1", result.Product);
    }

    [TestMethod]
    public void ReturnsTheCompanyFromTheSecondReader()
    {
      d1.Company = "";

      var result = sut.Read();

      Assert.AreEqual("c2", result.Company);
    }

    [TestMethod]
    public void ReturnsTheProductFromTheSecondReader()
    {
      d1.Product = "";

      var result = sut.Read();

      Assert.AreEqual("p2", result.Product);
    }
  }
}