using Challenge1.Library.Contracts;
using Challenge1.Library.Core;
using Challenge1.Library.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Challenge1.Tests.Shell
{
    [TestClass]
    public class MainLogicTests
    {
        private Mock<MainUI> ui;

        private MainLogic sut;

        [TestInitialize]
        public void SetUp()
        {
            ui = new Mock<MainUI>();

            sut = new MainLogic(ui.Object);
        }

        [TestMethod]
        public void EnteringADigitDisplaysThatDigit()
        {
            sut.EnterDigit('2');

            ui.Verify(it => it.Display("2"));
        }

        [TestMethod]
        public void EnteringSeveralDigitsDisplaysTheWholeNumber()
        {
            sut.EnterDigit('2');
            sut.EnterDigit('3');

            ui.Verify(it => it.Display("23"));
        }

        [TestMethod]
        public void EnteringAnOperatorDoesNotChangeTheDisplay()
        {
            sut.EnterDigit('2');
            sut.EnterDigit('3');
            sut.EnterOperator(new PlusOperator());

            ui.Verify(it => it.Display("23"));
        }

        [TestMethod]
        public void EnteringADigitAfterAnOperatorDisplaysThatDigit()
        {
            sut.EnterDigit('2');
            sut.EnterDigit('2');
            sut.EnterOperator(new PlusOperator());
            sut.EnterDigit('3');

            ui.Verify(it => it.Display("3"));
        }

        [TestMethod]
        public void EnteringTheEqualSignDisplaysTheResult_Plus()
        {
            sut.EnterDigit('2');
            sut.EnterDigit('2');
            sut.EnterOperator(new PlusOperator());
            sut.EnterDigit('3');
            sut.EnterDigit('3');
            sut.EnterEqual();

            ui.Verify(it => it.Display("55"));
        }

        [TestMethod]
        public void EnteringADigitAfterEqualDisplaysTheDigit()
        {
            sut.EnterDigit('2');
            sut.EnterOperator(new PlusOperator());
            sut.EnterDigit('2');
            sut.EnterEqual();
            sut.EnterDigit('3');

            ui.Verify(it => it.Display("3"));
        }
    }
}