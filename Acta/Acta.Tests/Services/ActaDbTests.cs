using System;
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
  }
}