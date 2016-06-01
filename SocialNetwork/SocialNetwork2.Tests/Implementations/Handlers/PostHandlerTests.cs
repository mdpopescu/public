using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations.Handlers;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations.Handlers
{
    [TestClass]
    public class PostHandlerTests
    {
        private PostHandler sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new PostHandler();
        }

        [TestMethod]
        public void HandlesTheArrowCommand()
        {
            Assert.AreEqual("->", sut.KnownCommand);
        }

        [TestMethod]
        public void CallsTheUserPostMethod()
        {
            var user = new Mock<IUser>();

            sut.Handle(user.Object, "a b");

            user.Verify(it => it.Post("a b"));
        }

        [TestMethod]
        public void ReturnsAnEmptySequence()
        {
            var user = new Mock<IUser>();

            var result = sut.Handle(user.Object, "a b").ToList();

            Assert.AreEqual(0, result.Count);
        }
    }
}