using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Tests.Implementations
{
    [TestClass]
    public class InputTerminalTests
    {
        private Mock<IUserRepository> userRepository;
        private Mock<IHandlerFactory> handlerFactory;

        private InputTerminal sut;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            handlerFactory = new Mock<IHandlerFactory>();
            sut = new InputTerminal(userRepository.Object, handlerFactory.Object);
        }

        [TestMethod]
        public void RetrievesTheUserBasedOnTheFirstPartOfTheInput()
        {
            var handler = new Mock<IHandler>();
            handlerFactory
                .Setup(it => it.GetHandler(It.IsAny<string>()))
                .Returns(handler.Object);

            sut.Handle("abc def ghi");

            userRepository.Verify(it => it.CreateOrFind("abc"));
        }

        [TestMethod]
        public void InvokesTheHandlerMatchingTheSecondPartOfTheInput()
        {
            var handler = new Mock<IHandler>();
            handlerFactory
                .Setup(it => it.GetHandler("def"))
                .Returns(handler.Object);

            sut.Handle("abc def ghi");

            handler.Verify(it => it.Handle(It.IsAny<IUser>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void ReturnsTheResultOfTheInvokedHandler()
        {
            var list = new[] { "zzz" };
            var handler = new Mock<IHandler>();
            handler
                .Setup(it => it.Handle(It.IsAny<User>(), "ghi"))
                .Returns(list);
            handlerFactory
                .Setup(it => it.GetHandler("def"))
                .Returns(handler.Object);

            var result = sut.Handle("abc def ghi").ToList();

            CollectionAssert.AreEqual(list, result);
        }
    }
}