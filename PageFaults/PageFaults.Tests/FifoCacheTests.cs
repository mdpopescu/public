using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.PageFaults.Tests
{
  [TestClass]
  public class FifoCacheTests
  {
    [TestMethod]
    public void GetPagesReturnsNullsInitially()
    {
      var sut = new FifoCache(3);

      var result = sut.Pages;

      Assert.IsTrue(result.All(it => it == null));
    }

    [TestMethod]
    public void AddingASinglePageReturnsThatPageFollowedByNulls()
    {
      var sut = new FifoCache(3);

      sut.AddPage(1);

      var result = sut.Pages;
      CollectionAssert.AreEqual(new int?[] { 1, null, null }, result);
    }

    [TestMethod]
    public void AddingAPageThatAlreadyExistsDoesNotChangeThePages()
    {
      var sut = new FifoCache(3);
      var pages = new[] { 0, 1, 2 };
      foreach (var page in pages)
        sut.AddPage(page);

      sut.AddPage(1);

      CollectionAssert.AreEqual(new[] { 2, 1, 0 }, sut.Pages);
    }

    [TestMethod]
    public void Complex()
    {
      var sut = new FifoCache(3);
      var pages = new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 };

      foreach (var page in pages)
        sut.AddPage(page);

      var result = sut.Pages;
      CollectionAssert.AreEqual(new[] { 2, 1, 3 }, result);
      Assert.AreEqual(16, sut.PageFaults);
    }
  }
}