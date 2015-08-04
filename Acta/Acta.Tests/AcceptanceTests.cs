using System;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;
using Acta.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acta.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void UsingTheLowLevelInterface()
    {
      IActaStorage storage = new ActaMemoryStorage();
      IActaLowLevelApi db = new ActaDb(storage);

      // create a new entity, writing each property separately
      var guid1 = Guid.NewGuid();
      db.Write(guid1, "@type", "Person"); // convention
      db.Write(guid1, "Name", "Marcel Popescu");
      db.Write(guid1, "DOB", new DateTime(1972, 4, 30));

      // create a new entity, writing all properties at the same time
      var guid2 = Guid.NewGuid();
      db.Write(guid2, new[]
      {
        new ActaKeyValuePair("@type", "Person"),
        new ActaKeyValuePair("Name", "Iolanda Popescu"),
        new ActaKeyValuePair("DOB", new DateTime(1974, 8, 31))
      });

      // change the names
      db.Write(guid1, "Name", "Marcel Doru Popescu");
      db.Write(guid2, "Name", "Dora Iolanda Popescu");

      // retrieve the guids of entities of type Person
      var guids = db.GetIds("@type", "Person").ToArray();

      // verify that the guids returned are correct
      CollectionAssert.AreEquivalent(new[] {guid1, guid2}, guids);

      // retrieve and verify the properties of the entities
      Assert.AreEqual("Marcel Doru Popescu", db.Read(guid1, "Name"));
      Assert.AreEqual(new DateTime(1972, 4, 30), db.Read(guid1, "DOB"));
      Assert.AreEqual("Dora Iolanda Popescu", db.Read(guid2, "Name"));
      Assert.AreEqual(new DateTime(1974, 8, 31), db.Read(guid2, "DOB"));
    }
  }
}