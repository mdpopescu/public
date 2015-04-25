using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TransformyClone.Library;

namespace TransformyClone.Tests
{
  [TestClass]
  public class ListTransformerTests
  {
    private Mock<Splitter> splitter;
    private Mock<Builder> builder;
    private ListTransformer sut;

    [TestInitialize]
    public void SetUp()
    {
      splitter = new Mock<Splitter>();
      builder = new Mock<Builder>();
      sut = new ListTransformer(splitter.Object, builder.Object);
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnNullInputList()
    {
      sut.Transform(null, "x");
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnEmptyInputList()
    {
      sut.Transform(new List<string>(), "x");
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnNullSample()
    {
      sut.Transform(new List<string> { "a" }, null);
    }

    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ThrowsOnEmptySample()
    {
      sut.Transform(new List<string> { "a" }, "");
    }

    [TestMethod]
    public void TransformsSingleElementToConstant()
    {
      splitter
        .Setup(it => it.Split("1"))
        .Returns(new[] { "1" });
      builder
        .Setup(it => it.Build(It.IsAny<string>(), "2", new[] { "1" }))
        .Returns("2");

      var result = sut.Transform(new[] { "1" }, "2");

      CollectionAssert.AreEqual(new[] { "2" }, result.ToArray());
    }

    [TestMethod]
    public void TransformsMultipleElementsToConstants()
    {
      splitter
        .Setup(it => it.Split("1"))
        .Returns(new[] { "1" });
      builder
        .Setup(it => it.Build(It.IsAny<string>(), "a", new[] { "1" }))
        .Returns("a");

      var result = sut.Transform(new[] { "1", "2", "3" }, "a");

      CollectionAssert.AreEqual(new[] { "a", "a", "a" }, result.ToArray());
    }

    [Ignore]
    [TestMethod]
    public void TransformsWhenSampleIncludesFirstItem()
    {
      var result = sut.Transform(new[] { "word", "thing" }, "some word and stuff");

      CollectionAssert.AreEqual(new[] { "some word and stuff", "some thing and stuff" }, result.ToArray());
    }

    [Ignore]
    [TestMethod]
    public void TransformsWhenSampleIncludesAllWordsFromFirstItem()
    {
      var result = sut.Transform(new[] { "a b", "dd eee" }, "a 1 b 2");

      CollectionAssert.AreEqual(new[] { "a 1 b 2", "dd 1 eee 2" }, result.ToArray());
    }
  }
}