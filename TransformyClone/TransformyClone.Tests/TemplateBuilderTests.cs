using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformyClone.Library;

namespace TransformyClone.Tests
{
  [TestClass]
  public class TemplateBuilderTests
  {
    private TemplateBuilder sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new TemplateBuilder();
    }

    [TestMethod]
    public void ReturnsUnchangedSampleWhenItDoesntMatchAnyWords()
    {
      var result = sut.Build("1 2 3", "abc", new[] { "1", "2", "3" });

      Assert.AreEqual("abc", result);
    }

    [TestMethod]
    public void ReplacesSingleMatchingWordWithPlaceholder()
    {
      var result = sut.Build("1 2 3", "a 1 b", new[] { "1", "2", "3" });

      Assert.AreEqual("a {0} b", result);
    }

    [TestMethod]
    public void ReplacesMultipleMatchingWords()
    {
      var result = sut.Build("1 2 3", "a 1 b 2", new[] { "1", "2", "3" });

      Assert.AreEqual("a {0} b {1}", result);
    }

    [TestMethod]
    public void ReplacesMultipleMatchingWordsWhenSkippingSome()
    {
      var result = sut.Build("1 2 3", "a 3 1 c", new[] { "1", "2", "3" });

      Assert.AreEqual("a {2} {0} c", result);
    }

    [TestMethod]
    public void DoublesOpeningCurlyBraces()
    {
      var result = sut.Build("1 2", "2 { 3", new[] { "1", "2" });

      Assert.AreEqual("{1} {{ 3", result);
    }
  }
}