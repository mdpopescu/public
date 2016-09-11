using Elomen.Library.Contracts;
using Elomen.Library.Implementations;
using Elomen.Library.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class InterpreterTests
    {
        private Mock<AccountRepository> accountRepository;
        private Mock<CommandParser> commandParser;

        private Interpreter sut;

        [TestInitialize]
        public void SetUp()
        {
            accountRepository = new Mock<AccountRepository>();
            commandParser = new Mock<CommandParser>();

            sut = new Interpreter(accountRepository.Object, commandParser.Object);
        }

        [TestClass]
        public class Execute : InterpreterTests
        {
            [TestMethod]
            public void LooksUpTheAccount()
            {
                sut.Execute("a", "b");

                accountRepository.Verify(it => it.Find("a"));
            }

            [TestMethod]
            public void ParsesTheCommand()
            {
                sut.Execute("a", "b");

                commandParser.Verify(it => it.Parse("b"));
            }

            [TestMethod]
            public void ReturnsAnErrorMessageIfItCannotParseTheCommand()
            {
                commandParser
                    .Setup(it => it.Parse("b"))
                    .Returns((Command) null);

                var result = sut.Execute("a", "b");

                Assert.AreEqual("I do not know what [b] means.", result);
            }

            [TestMethod]
            public void ExecutesTheCommand()
            {
                var account = new Account();
                accountRepository
                    .Setup(it => it.Find("a"))
                    .Returns(account);
                var command = new Mock<Command>();
                commandParser
                    .Setup(it => it.Parse("b"))
                    .Returns(command.Object);

                sut.Execute("a", "b");

                command.Verify(it => it.Execute(account));
            }

            [TestMethod]
            public void ReturnsTheResultOfTheCommand()
            {
                var command = new Mock<Command>();
                commandParser
                    .Setup(it => it.Parse("b"))
                    .Returns(command.Object);
                command
                    .Setup(it => it.Execute(It.IsAny<Account>()))
                    .Returns("message");

                var result = sut.Execute("a", "b");

                Assert.AreEqual("message", result);
            }
        }
    }
}