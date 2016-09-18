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
                sut.Execute(1, "x");

                accountRepository.Verify(it => it.Find(1));
            }

            [TestMethod]
            public void ParsesTheCommand()
            {
                sut.Execute(1, "x");

                commandParser.Verify(it => it.Parse("x"));
            }

            [TestMethod]
            public void ReturnsAnErrorMessageIfItCannotParseTheCommand()
            {
                commandParser
                    .Setup(it => it.Parse("x"))
                    .Returns((Command) null);

                var result = sut.Execute(1, "x");

                Assert.AreEqual("I do not know what [x] means.", result);
            }

            [TestMethod]
            public void ExecutesTheCommand()
            {
                accountRepository
                    .Setup(it => it.Find(1))
                    .Returns(Account.GUEST);
                var command = new Mock<Command>();
                commandParser
                    .Setup(it => it.Parse("x"))
                    .Returns(command.Object);

                sut.Execute(1, "x");

                command.Verify(it => it.Execute(Account.GUEST));
            }

            [TestMethod]
            public void ReturnsTheResultOfTheCommand()
            {
                var command = new Mock<Command>();
                commandParser
                    .Setup(it => it.Parse("x"))
                    .Returns(command.Object);
                command
                    .Setup(it => it.Execute(It.IsAny<Account>()))
                    .Returns("message");

                var result = sut.Execute(1, "x");

                Assert.AreEqual("message", result);
            }
        }
    }
}