using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.RecursiveCompare.Tests
{
  [TestClass]
  public class ComparisonTests
  {
    [TestMethod]
    public void ResultIsEqual()
    {
      var sut = new Comparison("test", 123, 123);

      Assert.AreEqual(ComparisonResult.Equal, sut.Result);
    }

    [TestMethod]
    public void ResultIsNotEqual()
    {
      var sut = new Comparison("test", "abc", "def");

      Assert.AreEqual(ComparisonResult.NotEqual, sut.Result);
    }
  }
}