using System;
using System.Linq;
using Acta.Library.Attributes;
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
    public void UsingTheLowLevelApi()
    {
      ActaStorage storage = new ActaMemoryStorage();
      ActaLowLevelApi db = new ActaDb(storage);

      // create a new entity, writing each property separately
      var guid1 = Guid.NewGuid();
      db.Write(guid1, "@type", "Person"); // convention
      db.Write(guid1, "Name", "Marcel Popescu");
      db.Write(guid1, "DOB", new DateTime(1972, 4, 30));

      // create a new entity, writing all properties at the same time
      var guid2 = Guid.NewGuid();
      db.Write(guid2,
        new ActaKeyValuePair("@type", "Person"),
        new ActaKeyValuePair("Name", "Iolanda Popescu"),
        new ActaKeyValuePair("DOB", new DateTime(1974, 8, 31)));

      // change the names
      db.Write(guid1, "Name", "Marcel Doru Popescu");
      db.Write(guid2, "Name", "Dora Iolanda Popescu");

      // retrieve the guids of entities of type Person
      var guids = db.GetIds("@type", "Person").ToArray();

      // verify that the guids returned are correct
      CollectionAssert.AreEquivalent(new[] {guid1, guid2}, guids);

      // retrieve and verify the properties of the entities
      Assert.AreEqual("Marcel Doru Popescu", (string) db.Read(guid1, "Name"));
      Assert.AreEqual(new DateTime(1972, 4, 30), db.Read(guid1, "DOB"));
      Assert.AreEqual("Dora Iolanda Popescu", db.Read<string>(guid2, "Name"));
      Assert.AreEqual(new DateTime(1974, 8, 31), db.Read(guid2, "DOB"));
    }

    [TestMethod]
    public void UsingTheEntityApi()
    {
      ActaStorage storage = new ActaMemoryStorage();
      ActaEntityApi db = new ActaDb(storage);

      // create a new entity
      var person1 = new Person {Name = "Marcel", DOB = new DateTime(1972, 4, 30)};
      db.AddOrUpdate(person1);

      // verify that the Id has been set
      Assert.AreNotEqual(Guid.Empty, person1.Id);

      // update an entity
      person1.Name = "Marcel Popescu";
      db.AddOrUpdate(person1);

      // retrieve the entity
      var person2 = db.Retrieve<Person>(person1.Id);
      Assert.AreEqual(person1.Name, person2.Name);
      Assert.AreEqual(person1.DOB, person2.DOB);
    }

    //

    private class Person
    {
      // convention: the key is
      // - the first property marked with the [Key] attribute
      // - the Id property
      // - the {type}Id property

      [Key]
      public Guid Id { get; set; }

      public string Name { get; set; }
      public DateTime DOB { get; set; }
    }
  }
}