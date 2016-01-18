using System.Reactive.Concurrency;
using System.Reactive.Linq;
using BigDataProcessing.Library.Services;
using BigDataProcessing.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataProcessing.Tests.Services
{
  [TestClass]
  public class RxGenericSplitterTests
  {
    private RxGenericSplitter<int> sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new RxGenericSplitter<int>();
    }

    [TestMethod]
    public void ReturnsTheSameStream()
    {
      var input = new[] { 1, 2, 3 };

      var result = sut.Split(input.ToObservable(), 1, Scheduler.Immediate);

      Assert.AreEqual(1, result.Length);
      CollectionAssert.AreEqual(input, Extensions.ToList(result[0]));
    }

    [TestMethod]
    public void ReturnsTwoStreams()
    {
      var input = new[] { 1, 2, 3, 4 };

      var result = sut.Split(input.ToObservable(), 2, Scheduler.Immediate);

      Assert.AreEqual(2, result.Length);
      CollectionAssert.AreEqual(new[] { 1, 3 }, Extensions.ToList(result[0]));
      CollectionAssert.AreEqual(new[] { 2, 4 }, Extensions.ToList(result[1]));
    }
  }
}