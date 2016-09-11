using Elomen.Library.Contracts;
using Elomen.Library.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class InterpreterTests
    {
        private Mock<AccountRepository> accountRepository;

        private Interpreter sut;

        [TestInitialize]
        public void SetUp()
        {
            accountRepository = new Mock<AccountRepository>();

            sut = new Interpreter(accountRepository.Object);
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
        }
    }
}