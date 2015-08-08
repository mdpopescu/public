using System;
using Acta.Library;
using Acta.Library.Contracts;
using Acta.Library.Models;
using Acta.Library.Services;
using Acta.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Acta.Tests.Services
{
  [TestClass]
  public class ActaEntityLayerTests
  {
    private Mock<ActaLowLevelApi> db;
    private ActaEntityLayer sut;

    [TestInitialize]
    public void SetUp()
    {
      db = new Mock<ActaLowLevelApi>();
      sut = new ActaEntityLayer(db.Object);
    }

    [TestClass]
    public class AddOrUpdate : ActaEntityLayerTests
    {
      [TestMethod]
      public void WritesTheTypeAndPublicProperties()
      {
        ActaKeyValuePair[] result = null;
        db
          .Setup(it => it.Write(It.IsAny<Guid>(), It.IsAny<ActaKeyValuePair[]>()))
          .Callback<Guid, ActaKeyValuePair[]>((g, a) => result = a);

        sut.AddOrUpdate(new Person {Name = "Marcel", DOB = new DateTime(1972, 4, 30)});

        Assert.AreEqual(3, result.Length);
        Assert.AreEqual(Global.TYPE_KEY, result[0].Name);
        Assert.AreEqual("Acta.Tests.Helper.Person", result[0].Value);
        Assert.AreEqual("Name", result[1].Name);
        Assert.AreEqual("Marcel", result[1].Value);
        Assert.AreEqual("DOB", result[2].Name);
        Assert.AreEqual(new DateTime(1972, 4, 30), result[2].Value);
      }

      [TestMethod]
      public void WritesNothingIfTheEntityIsNull()
      {
        sut.AddOrUpdate(null);

        db.Verify(it => it.Write(It.IsAny<Guid>(), It.IsAny<ActaKeyValuePair[]>()), Times.Never);
        db.Verify(it => it.Write(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
      }

      [TestMethod]
      public void ReturnsAGuid()
      {
        var person = new Person();

        var result = sut.AddOrUpdate(person);

        Assert.AreNotEqual(Guid.Empty, result);
      }

      [TestMethod]
      public void ReturnsTheEmptyGuidIfTheEntityIsNull()
      {
        var result = sut.AddOrUpdate(null);

        Assert.AreEqual(Guid.Empty, result);
      }
    }
  }
}