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
        private Mock<CommandParser> commandParser;

        private Interpreter sut;

        private Account account;

        [TestInitialize]
        public void SetUp()
        {
            commandParser = new Mock<CommandParser>();

            sut = new Interpreter(commandParser.Object);

            account = new Account(1, "a");
        }

        [TestClass]
        public class Execute : InterpreterTests
        {
            [TestMethod]
            public void ParsesTheCommand()
            {
                sut.Execute(account, "x");

                commandParser.Verify(it => it.Parse("x"));
            }

            [TestMethod]
            public void ReturnsAnErrorMessageIfItCannotParseTheCommand()
            {
                commandParser
                    .Setup(it => it.Parse("x"))
                    .Returns((Command) null);

                var result = sut.Execute(account, "x");

                Assert.AreEqual("I do not know what [x] means.", result);
            }

            [TestMethod]
            public void ExecutesTheCommand()
            {
                var command = new Mock<Command>();
                commandParser
                    .Setup(it => it.Parse("x"))
                    .Returns(command.Object);

                sut.Execute(account, "x");

                command.Verify(it => it.Execute(account));
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

                var result = sut.Execute(account, "x");

                Assert.AreEqual("message", result);
            }
        }
    }
}