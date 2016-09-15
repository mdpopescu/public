using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations.Handlers
{
    [TestClass]
    public class ReadHandlerTests
    {
        private ReadHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new ReadHandler();
        }

        [TestMethod]
        public void CallsTheUserReadMethod()
        {
            var user = new Mock<IUser>();

            sut.Handle(user.Object, "");

            user.Verify(it => it.Read());
        }

        [TestMethod]
        public void ReturnsTheResultOfTheUserReadMethod()
        {
            var user = new Mock<IUser>();
            // ReSharper disable once CollectionNeverUpdated.Local
            var messages = new List<string>();
            user
                .Setup(it => it.Read())
                .Returns(messages);

            var result = sut.Handle(user.Object, "");

            Assert.AreEqual(messages, result);
        }
    }
}