using Challenge1.Library.Contracts;
using Challenge1.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge1.Tests.Core
{
    [TestClass]
    public class PostOperatorStateTests
    {
        private CalculatorState sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new PostOperatorState("22", new PlusOperator());
        }

        [TestMethod]
        public void EnteringADigitDisplaysThatDigit()
        {
            sut = sut.EnterDigit('2');

            Assert.AreEqual("2", sut.Display);
        }

        [TestMethod]
        public void EnteringTheEqualSignDisplaysTheResult()
        {
            sut = sut.EnterDigit('3');
            sut = sut.EnterDigit('3');
            sut = sut.EnterEqual();

            Assert.AreEqual("55", sut.Display);
        }
    }
}