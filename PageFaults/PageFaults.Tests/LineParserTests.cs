using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.PageFaults.Tests
{
  [TestClass]
  public class LineParserTests
  {
    private const string ALL_BUT_TYPE = ",3,0,1,2,3,2,3,0,4,5,2,3,1,4,3,2,6,3,2,1,2";

    private LineParser sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new LineParser();
    }

    [TestMethod]
    public void ParseFifo()
    {
      var result = sut.Parse("F" + ALL_BUT_TYPE);

      Assert.IsTrue(result.Cache is FifoCache);
      Assert.AreEqual(3, result.Cache.CacheSize);
      CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 }, result.Pages);
    }

    [TestMethod]
    public void ParseLru()
    {
      var result = sut.Parse("L" + ALL_BUT_TYPE);

      Assert.IsTrue(result.Cache is LruCache);
      Assert.AreEqual(3, result.Cache.CacheSize);
      CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 }, result.Pages);
    }

    [TestMethod]
    public void ParseOptimal()
    {
      var result = sut.Parse("O" + ALL_BUT_TYPE);

      Assert.IsTrue(result.Cache is OptimalCache);
      Assert.AreEqual(3, result.Cache.CacheSize);
      CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 }, result.Pages);
      CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 2, 3, 0, 4, 5, 2, 3, 1, 4, 3, 2, 6, 3, 2, 1, 2 }, ((OptimalCache) result.Cache).Future);
    }
  }
}