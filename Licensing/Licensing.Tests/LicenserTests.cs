using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests
{
  [TestClass]
  public class LicenserTests
  {
    private Licenser sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new Licenser();
    }

    [TestClass]
    public class Check : LicenserTests
    {
      [TestMethod]
      public void TestMethod1()
      {
      }
    }
  }
}