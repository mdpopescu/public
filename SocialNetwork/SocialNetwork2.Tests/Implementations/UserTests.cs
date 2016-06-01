using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Tests.Implementations
{
    [TestClass]
    public class UserTests
    {
        private User sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new User("abc");
        }

        [TestMethod]
        public void ReadingWithoutPostingReturnsAnEmptySequence()
        {
            var result = sut.Read().ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void PostedMessagesAreReturnedByRead()
        {
            sut.Post("test");

            var result = sut.Read().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].StartsWith("test"));
        }

        [TestMethod]
        public void TheMessagesAreTaggedWithTheElapsedTime()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Post("test");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 10);
            var result = sut.Read().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("test (5 seconds ago)", result[0]);
        }

        [TestMethod]
        public void TheMessagesAreReturnedInReverseOrder()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Post("abc");
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 10);
            sut.Post("def");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 15);
            var result = sut.Read().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("def (5 seconds ago)", result[0]);
            Assert.AreEqual("abc (10 seconds ago)", result[1]);
        }

        [TestMethod]
        public void TheWallReturnsOwnMessagesPrefixedWithName()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Post("test");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 10);
            var result = sut.Wall().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("abc - test (5 seconds ago)", result[0]);
        }
    }
}