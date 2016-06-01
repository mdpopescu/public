using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations.Handlers
{
    [TestClass]
    public class WallHandlerTests
    {
        private WallHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new WallHandler();
        }

        [TestMethod]
        public void HandlesTheWallCommand()
        {
            Assert.AreEqual("wall", sut.KnownCommand);
        }

        [TestMethod]
        public void CallsTheUserReadMethod()
        {
            var user = new Mock<IUser>();

            sut.Handle(user.Object, "");

            user.Verify(it => it.Wall());
        }

        [TestMethod]
        public void ReturnsTheResultOfTheUserReadMethod()
        {
            var user = new Mock<IUser>();
            // ReSharper disable once CollectionNeverUpdated.Local
            var messages = new List<string>();
            user
                .Setup(it => it.Wall())
                .Returns(messages);

            var result = sut.Handle(user.Object, "");

            Assert.AreEqual(messages, result);
        }
    }
}