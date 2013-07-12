using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.PageFaults.Tests
{
  [TestClass]
  public class LruCacheTests
  {
    [TestMethod]
    public void Complex()
    {
      var sut = new LruCache(3);
      var pages = new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 };

      foreach (var page in pages)
        sut.AddPage(page);

      var result = sut.Pages;
      CollectionAssert.AreEqual(new[] { 2, 1, 3 }, result);
      Assert.AreEqual(14, sut.PageFaults);
    }
  }
}