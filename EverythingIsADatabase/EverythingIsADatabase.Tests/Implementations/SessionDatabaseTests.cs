using System.Linq;
using EverythingIsADatabase.Logic.Implementations;
using EverythingIsADatabase.Logic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EverythingIsADatabase.Tests.Implementations
{
    [TestClass]
    public class SessionDatabaseTests
    {
        private SessionDatabase sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new SessionDatabase();
        }

        [TestMethod]
        public void SearchingByName()
        {
            var record = new Record(new Attribute("firstname", "Marcel"), new Attribute("age", 44));

            sut.Commit(record);

            // finds by name
            var r1 = sut.Search(new Attribute("firstname")).ToList();
            Assert.AreEqual(1, r1.Count);

            // does not find by name
            var r2 = sut.Search(new Attribute("lastname")).ToList();
            Assert.AreEqual(0, r2.Count);

            // finds by multiple names
            var r3 = sut.Search(new Attribute("firstname"), new Attribute("age")).ToList();
            Assert.AreEqual(1, r3.Count);

            // does not find if some names exist but others don't
            var r4 = sut.Search(new Attribute("firstname"), new Attribute("lastname")).ToList();
            Assert.AreEqual(0, r4.Count);
        }

        [TestMethod]
        public void SearchingByNameAndValue()
        {
            var record = new Record(new Attribute("firstname", "Marcel"), new Attribute("age", 44));

            sut.Commit(record);

            // finds by single name and value
            var r1 = sut.Search(new Attribute("firstname", "Marcel")).ToList();
            Assert.AreEqual(1, r1.Count);

            // does not find if the value doesn't match
            var r2 = sut.Search(new Attribute("firstname", "Gigi")).ToList();
            Assert.AreEqual(0, r2.Count);

            // finds by multiple names and values
            var r3 = sut.Search(new Attribute("firstname", "Marcel"), new Attribute("age", 44)).ToList();
            Assert.AreEqual(1, r3.Count);

            // does not find if some values don't match
            var r4 = sut.Search(new Attribute("firstname", "Marcel"), new Attribute("age", 30)).ToList();
            Assert.AreEqual(0, r4.Count);
        }
    }
}