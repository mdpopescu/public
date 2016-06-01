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
            Assert.AreEqual("test", result[0]);
        }
    }
}