using System;
using System.Linq;
using Acta.Library;
using Acta.Library.Contracts;
using Acta.Library.Models;
using Acta.Library.Services;
using Acta.Tests.Helper;
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
            db.Write(guid1, Global.TYPE_KEY, "Person"); // convention
            db.Write(guid1, "Name", "Marcel Popescu");
            db.Write(guid1, "DOB", new DateTime(1972, 4, 30));

            // create a new entity, writing all properties at the same time
            var guid2 = Guid.NewGuid();
            db.Write(guid2,
                new ActaKeyValuePair(Global.TYPE_KEY, "Person"),
                new ActaKeyValuePair("Name", "Iolanda Popescu"),
                new ActaKeyValuePair("DOB", new DateTime(1974, 8, 31)));

            // change the names
            db.Write(guid1, "Name", "Marcel Doru Popescu");
            db.Write(guid2, "Name", "Dora Iolanda Popescu");

            // retrieve the guids of entities of type Person
            var guids = db.GetIds(Global.TYPE_KEY, "Person").ToArray();

            // verify that the guids returned are correct
            CollectionAssert.AreEquivalent(new[] { guid1, guid2 }, guids);

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
            ActaLowLevelApi lowLevel = new ActaDb(storage);
            ActaEntityApi db = new ActaEntityLayer(lowLevel);

            // create a new entity
            var person = new Person { Name = "Marcel", DOB = new DateTime(1972, 4, 30) };
            var guid = db.Store(person);

            // verify that the GUID is valid
            Assert.AreNotEqual(Guid.Empty, guid);

            // update an entity
            person.Name = "Marcel Popescu";
            db.Store(person);

            // retrieve the entity as a list of key/value pairs
            var pairs = db.Fetch(guid).ToList();
            Assert.AreEqual(Global.TYPE_KEY, pairs[0].Name);
            Assert.AreEqual("Acta.Tests.Helper.Person", pairs[0].Value);
            Assert.AreEqual("ID", pairs[1].Name);
            Assert.AreEqual("NAME", pairs[2].Name);
            Assert.AreEqual("Marcel Popescu", pairs[2].Value);
            Assert.AreEqual("DOB", pairs[3].Name);
            Assert.AreEqual(new DateTime(1972, 4, 30), pairs[3].Value);

            // retrieve the entity as an object
            var person2 = db.Fetch<Person>(guid);
            Assert.AreEqual(person.Name, person2.Name);
            Assert.AreEqual(person.DOB, person2.DOB);
        }
    }
}