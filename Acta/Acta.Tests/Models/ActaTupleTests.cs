using System;
using Acta.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acta.Tests.Models
{
  [TestClass]
  public class ActaTupleTests
  {
    [TestMethod]
    [ExpectedException(typeof (ArgumentException))]
    public void ConstructorRejectsInvalidPropertyName()
    {
      var temp = new ActaTuple(Guid.NewGuid(), "  ", "value");
    }

    [TestMethod]
    public void MatchesSucceedsOnGuidAndName()
    {
      var guid = Guid.NewGuid();
      var sut = new ActaTuple(guid, "test", "value");

      var result = sut.Matches(guid, "test");

      Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesFailsOnGuidAndName()
    {
      var guid = Guid.NewGuid();
      var sut = new ActaTuple(guid, "test1", "value");

      var result = sut.Matches(guid, "test2");

      Assert.IsFalse(result);
    }

    [TestMethod]
    public void Matches_NameIsCaseInsensitive()
    {
      var guid = Guid.NewGuid();
      var sut = new ActaTuple(guid, "test", "value");

      var result = sut.Matches(guid, "TEST");

      Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesSucceedsOnNameAndValue()
    {
      var sut = new ActaTuple(Guid.NewGuid(), "test", "value");

      var result = sut.Matches("test", "value");

      Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchesFailsOnNameAndValue()
    {
      var sut = new ActaTuple(Guid.NewGuid(), "test", "value1");

      var result = sut.Matches("test", "value2");

      Assert.IsFalse(result);
    }
  }
}