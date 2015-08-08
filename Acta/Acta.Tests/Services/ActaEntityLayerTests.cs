using System;
using Acta.Library;
using Acta.Library.Contracts;
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
      public void AddsTheTypeTuple()
      {
        sut.AddOrUpdate(new Person());

        db.Verify(it => it.Write(It.IsAny<Guid>(), Global.TYPE_KEY, "Acta.Tests.Helper.Person"));
      }
    }
  }
}