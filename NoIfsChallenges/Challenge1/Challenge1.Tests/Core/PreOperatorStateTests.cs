using Challenge1.Library.Contracts;
using Challenge1.Library.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge1.Tests.Core
{
    [TestClass]
    public class PreOperatorStateTests
    {
        private CalculatorState sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new PreOperatorState();
        }

        [TestMethod]
        public void EnteringADigitDisplaysThatDigit()
        {
            sut = sut.EnterDigit('2');

            Assert.AreEqual("2", sut.Display);
        }

        [TestMethod]
        public void EnteringSeveralDigitsDisplaysTheWholeNumber()
        {
            sut = sut.EnterDigit('2');
            sut = sut.EnterDigit('3');

            Assert.AreEqual("23", sut.Display);
        }

        [TestMethod]
        public void EnteringAnOperatorChangesTheStateToPostOperator()
        {
            sut = sut.EnterDigit('2');
            sut = sut.EnterOperator(new PlusOperator());

            Assert.IsInstanceOfType(sut, typeof(PostOperatorState));
        }
    }
}