using ISBN.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISBN.Tests
{
  [TestClass]
  public class ConverterTests
  {
    [TestMethod]
    public void Test1()
    {
      var sut = new Converter();

      var result = sut.Convert("978155192370");

      Assert.AreEqual("155192370x", result);
    }
  }
}