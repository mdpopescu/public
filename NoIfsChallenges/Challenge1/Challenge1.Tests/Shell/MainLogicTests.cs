using Challenge1.Library.Contracts;
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
    }
}