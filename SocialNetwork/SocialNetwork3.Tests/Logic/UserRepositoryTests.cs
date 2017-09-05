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
        public void ReturnsTheFollowed()
        {
            sut.AddFollower("z", "a");
            sut.AddFollower("z", "b");

            var result = sut.GetFollowed("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }

        [TestMethod]
        public void DoesNotAddTheSameFollowerTwice()
        {
            sut.AddFollower("z", "a");
            sut.AddFollower("z", "b");
            sut.AddFollower("z", "a");

            var result = sut.GetFollowed("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }

        [TestMethod]
        public void DoesNotAddAUserAsFollowerToHimself()
        {
            sut.AddFollower("z", "a");
            sut.AddFollower("z", "b");
            sut.AddFollower("z", "z");

            var result = sut.GetFollowed("z").ToArray();

            CollectionAssert.AreEqual(new[] { "a", "b" }, result);
        }
    }
}