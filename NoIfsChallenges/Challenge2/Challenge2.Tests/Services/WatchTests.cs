using Challenge2.Library.Contracts;
using Challenge2.Library.Services;
using Challenge2.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Challenge2.Tests.Services
{
    [TestClass]
    public class WatchTests
    {
        private Mock<UserInterface> ui;

        private Watch sut;

        private TestTimer timer;

        [TestInitialize]
        public void SetUp()
        {
            ui = new Mock<UserInterface>();

            sut = new Watch(ui.Object);

            timer = new TestTimer();
            GlobalSettings.Timer = timer;
        }

        [TestClass]
        public class Initialize : WatchTests
        {
            [TestMethod]
            public void InitializesTheUI()
            {
                sut.Initialize();

                ui.VerifySet(it => it.StartStopEnabled = true);
                ui.VerifySet(it => it.ResetEnabled = false);
                ui.VerifySet(it => it.HoldEnabled = false);
                ui.VerifySet(it => it.Text = "00:00:00");
            }
        }

        [TestClass]
        public class StartStop : WatchTests
        {
            [TestMethod]
            public void UpdatesTheUIState()
            {
                sut.StartStop();

                ui.VerifySet(it => it.StartStopEnabled = true);
                ui.VerifySet(it => it.ResetEnabled = false);
                ui.VerifySet(it => it.HoldEnabled = true);
                ui.VerifySet(it => it.Text = "00:00:00");
            }

            [TestMethod]
            public void UpdatesTheClockWhenASecondPasses()
            {
                sut.StartStop();

                timer.Advance(1);

                ui.VerifySet(it => it.Text = "00:00:01");
            }
        }
    }
}