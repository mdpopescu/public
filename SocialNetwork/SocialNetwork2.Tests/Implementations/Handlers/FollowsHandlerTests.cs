using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations.Handlers
{
    [TestClass]
    public class FollowsHandlerTests
    {
        private Mock<IUserRepository> userRepository;

        private FollowsHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();

            sut = new FollowsHandler(userRepository.Object);
        }

        [TestMethod]
        public void HandlesTheFollowsCommand()
        {
            Assert.AreEqual("follows", sut.KnownCommand);
        }

        [TestMethod]
        public void RetrievesTheOtherUserFromTheRepository()
        {
            sut.Handle(new User(""), "a");

            userRepository.Verify(it => it.CreateOrFind("a"));
        }

        [TestMethod]
        public void CallsTheUserFollowsMethod()
        {
            var user = new Mock<IUser>();
            var other = new Mock<IUser>();
            userRepository
                .Setup(it => it.CreateOrFind("a"))
                .Returns(other.Object);

            sut.Handle(user.Object, "a");

            user.Verify(it => it.Follow(other.Object));
        }

        [TestMethod]
        public void ReturnsAnEmptySequence()
        {
            var user = new Mock<IUser>();

            var result = sut.Handle(user.Object, "a").ToList();

            Assert.AreEqual(0, result.Count);
        }
    }
}