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
    }
}