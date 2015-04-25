using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformyClone.Library;

namespace TransformyClone.Tests
{
  [TestClass]
  public class WordSplitterTests
  {
    private WordSplitter sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new WordSplitter();
    }

    [TestMethod]
    public void ReturnsInputWhenSingleWord()
    {
      var result = sut.Split("test").ToList();

      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("test", result[0]);
    }

    [TestMethod]
    public void ReturnsSingleWordWithoutSpaces()
    {
      var result = sut.Split("  test   ").ToList();

      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("test", result[0]);
    }

    [TestMethod]
    public void ReturnsEmptyListForNull()
    {
      var result = sut.Split(null).ToList();

      Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ReturnsTwoWords()
    {
      var result = sut.Split(" a b").ToList();

      Assert.AreEqual(2, result.Count);
      Assert.AreEqual("a", result[0]);
      Assert.AreEqual("b", result[1]);
    }

    [TestMethod]
    public void ReturnsNonSpaceCharactersAsWords()
    {
      var result = sut.Split("a->b c>d").ToList();

      Assert.AreEqual(6, result.Count);
      Assert.AreEqual("a", result[0]);
      Assert.AreEqual("->", result[1]);
      Assert.AreEqual("b", result[2]);
      Assert.AreEqual("c", result[3]);
      Assert.AreEqual(">", result[4]);
      Assert.AreEqual("d", result[5]);
    }
  }
}