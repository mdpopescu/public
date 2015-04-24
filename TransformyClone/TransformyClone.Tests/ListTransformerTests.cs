using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransformyClone.Library;

namespace TransformyClone.Tests
{
  [TestClass]
  public class ListTransformerTests
  {
    private ListTransformer sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new ListTransformer();
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
      var result = sut.Transform(new[] { "1" }, "2");

      CollectionAssert.AreEqual(new[] { "2" }, result.ToArray());
    }

    [TestMethod]
    public void TransformsMultipleElementsToConstants()
    {
      var result = sut.Transform(new[] { "1", "2", "3" }, "a");

      CollectionAssert.AreEqual(new[] { "a", "a", "a" }, result.ToArray());
    }
  }
}