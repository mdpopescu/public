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
        private Mock<IHandler> handler1;
        private Mock<IHandler> handler2;

        private InputTerminal sut;

        [TestInitialize]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            handler1 = new Mock<IHandler>();
            handler2 = new Mock<IHandler>();
            sut = new InputTerminal(userRepository.Object, new[] { handler1.Object, handler2.Object });
        }

        [TestMethod]
        public void AsksEachHandlerForTheCommandItKnows()
        {
            sut.Handle("stuff");

            handler1.Verify(it => it.KnownCommand);
            handler2.Verify(it => it.KnownCommand);
        }

        [TestMethod]
        public void RetrievesTheUserBasedOnTheFirstPartOfTheInput()
        {
            sut.Handle("abc def ghi");

            userRepository.Verify(it => it.CreateOrFind("abc"));
        }

        [TestMethod]
        public void InvokesTheHandlerMatchingTheCommandInTheSecondPosition()
        {
            handler1
                .Setup(it => it.KnownCommand)
                .Returns("def");

            sut.Handle("abc def ghi");

            handler1.Verify(it => it.Handle(It.IsAny<IUser>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void DoesNotInvokeHandlersThatDoNotMatchTheCommand()
        {
            handler1
                .Setup(it => it.KnownCommand)
                .Returns("def");
            handler2
                .Setup(it => it.KnownCommand)
                .Returns("zzz");

            sut.Handle("abc def ghi");

            handler2.Verify(it => it.Handle(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void OnlyInvokesTheFirstMatchingHandler()
        {
            handler1
                .Setup(it => it.KnownCommand)
                .Returns("def");
            handler2
                .Setup(it => it.KnownCommand)
                .Returns("def");

            sut.Handle("abc def ghi");

            handler2.Verify(it => it.Handle(It.IsAny<IUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ReturnsTheResultOfTheInvokedHandler()
        {
            handler1
                .Setup(it => it.KnownCommand)
                .Returns("def");
            var list = new[] { "zzz" };
            handler1
                .Setup(it => it.Handle(It.IsAny<User>(), "ghi"))
                .Returns(list);

            var result = sut.Handle("abc def ghi").ToList();

            CollectionAssert.AreEqual(list, result);
        }
    }
}