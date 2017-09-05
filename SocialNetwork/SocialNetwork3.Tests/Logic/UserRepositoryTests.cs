using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Tests.Logic
{
    [TestClass]
    public class UserRepositoryTests
    {
        private UserRepository sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new UserRepository();
        }

        [TestMethod]
        public void ReturnsTheFollowers()
        {
            sut.AddFollower("a", "z");
            sut.AddFollower("b", "z");

            var result = sut.GetFollowers("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }

        [TestMethod]
        public void DoesNotAddTheSameFollowerTwice()
        {
            sut.AddFollower("a", "z");
            sut.AddFollower("b", "z");
            sut.AddFollower("a", "z");

            var result = sut.GetFollowers("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }

        [TestMethod]
        public void DoesNotAddAUserAsFollowerToHimself()
        {
            sut.AddFollower("a", "z");
            sut.AddFollower("b", "z");
            sut.AddFollower("z", "z");

            var result = sut.GetFollowers("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }
    }
}