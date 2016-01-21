using System.Reactive.Linq;
using System.Reactive.Subjects;
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

      var result = sut.Split(input.GetEnumerator(), 1);

      Assert.AreEqual(1, result.Length);
      var result0 = Extensions.ToList(result[0]);
      CollectionAssert.AreEqual(input, result0);
    }

    [TestMethod]
    public void ReturnsTwoStreams()
    {
      var input = new[] { 1, 2, 3, 4 };

      var result = sut.Split(input.GetEnumerator(), 2);

      Assert.AreEqual(2, result.Length);

      var result0 = Extensions.ToList(result[0]);
      var result1 = Extensions.ToList(result[1]);
      CollectionAssert.AreEqual(new[] { 1, 3 }, result0);
      CollectionAssert.AreEqual(new[] { 2, 4 }, result1);
    }

    [TestMethod]
    public void ReturnsTwoStreamsFromValuesGeneratedDynamically()
    {
      var input = new Subject<int>();

      var result = sut.Split(input.GetEnumerator(), 2);
      input.OnNext(1);
      input.OnNext(2);
      input.OnNext(3);
      input.OnNext(4);
      input.OnCompleted();

      Assert.AreEqual(2, result.Length);

      var result0 = Extensions.ToList(result[0]);
      var result1 = Extensions.ToList(result[1]);
      CollectionAssert.AreEqual(new[] { 1, 3 }, result0);
      CollectionAssert.AreEqual(new[] { 2, 4 }, result1);
    }
  }
}