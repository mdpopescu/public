using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Tests.Implementations
{
    [TestClass]
    public class UserRepositoryTests
    {
        private UserRepository sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new UserRepository(name => new User(name));
        }

        [TestMethod]
        public void ReturnsAUserInstance()
        {
            var result = sut.CreateOrFind("abc");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(User));
        }

        [TestMethod]
        public void ReturnsTheSameInstanceOnSubsequentCalls()
        {
            var user1 = sut.CreateOrFind("abc");
            var user2 = sut.CreateOrFind("abc");

            Assert.AreEqual(user1, user2);
        }
    }
}