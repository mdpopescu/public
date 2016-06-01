using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations
{
    [TestClass]
    public class InputHandlerTests
    {
        private UserRepository userRepository;

        private InputHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new UserRepository(name => new User(name));
            var handler = new Mock<IHandler>();
            sut = new InputHandler(userRepository, new[] { handler.Object });
        }

        [TestMethod]
        public void PostingAddsMessagesToUser()
        {
            sut.Handle("abc -> test");

            var user = userRepository.CreateOrFind("abc");
            var messages = user.Read().ToList();
            Assert.AreEqual(1, messages.Count);
            Assert.IsTrue(messages[0].StartsWith("test"));
        }

        [TestMethod]
        public void ReadingReturnsMessagesFromTheUser()
        {
            sut.Handle("abc -> test");

            var result = sut.Handle("abc").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].StartsWith("test"));
        }

        [TestMethod]
        public void ReturnsMessagesFromTheUserWall()
        {
            sut.Handle("abc -> test");

            var result = sut.Handle("abc wall").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].StartsWith("abc - test"));
        }

        [TestMethod]
        public void FollowsOtherUsers()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Handle("abc -> test1");
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 6);
            sut.Handle("def -> test2");

            sut.Handle("abc follows def");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 10);
            var result = sut.Handle("abc wall").ToList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("def - test2 (4 seconds ago)", result[0]);
            Assert.AreEqual("abc - test1 (5 seconds ago)", result[1]);
        }
    }
}