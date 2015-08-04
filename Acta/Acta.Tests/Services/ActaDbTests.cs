using System;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;
using Acta.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Acta.Tests.Services
{
  [TestClass]
  public class ActaDbTests
  {
    private Mock<IActaStorage> storage;
    private ActaDb sut;

    [TestInitialize]
    public void SetUp()
    {
      storage = new Mock<IActaStorage>();
      sut = new ActaDb(storage.Object);
    }

    [TestClass]
    public class WriteOne : ActaDbTests
    {
      [TestMethod]
      public void WritesTupleToStorage()
      {
        var guid = Guid.NewGuid();

        sut.Write(guid, "test", "value");

        storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "test" && (string) t.Value == "value")));
      }
    }

    [TestClass]
    public class WriteMany : ActaDbTests
    {
      [TestMethod]
      public void WritesMultipleTuplesToStorage()
      {
        var guid = Guid.NewGuid();

        sut.Write(guid,
          new ActaKeyValuePair("k1", "v1"),
          new ActaKeyValuePair("k2", "v2"));

        storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "k1" && (string) t.Value == "v1")));
        storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "k2" && (string) t.Value == "v2")));
      }
    }

    [TestClass]
    public class GetIds : ActaDbTests
    {
      [TestMethod]
      public void ReturnsTheGuidForTheMatchingEntity()
      {
        var guid = Guid.NewGuid();
        storage
          .Setup(it => it.Get())
          .Returns(new[]
          {
            new ActaTuple(guid, "test", "value", 0),
          });

        var result = sut.GetIds("test", "value").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(guid, result[0]);
      }

      [TestMethod]
      public void ReturnsGuidsForAllMatchingEntities()
      {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        storage
          .Setup(it => it.Get())
          .Returns(new[]
          {
            new ActaTuple(guid1, "test", "value", 0),
            new ActaTuple(guid2, "test", "value", 0),
          });

        var result = sut.GetIds("test", "value").ToArray();

        CollectionAssert.AreEquivalent(new[] {guid1, guid2}, result);
      }

      [TestMethod]
      public void DoesNotReturnTheGuidsOfNonMatchingEntities()
      {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        storage
          .Setup(it => it.Get())
          .Returns(new[]
          {
            new ActaTuple(guid1, "test", "value", 0),
            new ActaTuple(guid2, "test", "wrong value", 0),
            new ActaTuple(guid3, "test", "value", 0),
          });

        var result = sut.GetIds("test", "value").ToArray();

        CollectionAssert.AreEquivalent(new[] {guid1, guid3}, result);
      }
    }

    [TestClass]
    public class ReadObject : ActaDbTests
    {
      [TestMethod]
      public void ReturnsMatchingValue()
      {
        var guid = Guid.NewGuid();
        storage
          .Setup(it => it.Get())
          .Returns(new[]
          {
            new ActaTuple(guid, "test", "value", 0),
          });

        var result = sut.Read(guid, "test") as string;

        Assert.AreEqual("value", result);
      }

      [TestMethod]
      public void ReturnsNullIfPropertyNotFoundForTheGivenId()
      {
        var result = sut.Read(Guid.NewGuid(), "test");

        Assert.IsNull(result);
      }
    }
  }
}