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
                ui.VerifySet(it => it.Display = "00:00:00");
            }
        }

        [TestClass]
        public class StartStop : WatchTests
        {
            [TestClass]
            public class Starting : StartStop
            {
                [TestInitialize]
                public void InnerSetUp()
                {
                    sut.Initialize();
                }

                [TestMethod]
                public void UpdatesTheUIState()
                {
                    sut.StartStop();

                    ui.VerifySet(it => it.StartStopEnabled = true);
                    ui.VerifySet(it => it.ResetEnabled = false);
                    ui.VerifySet(it => it.HoldEnabled = true);
                }

                [TestMethod]
                public void UpdatesTheClockWhenASecondPasses()
                {
                    sut.StartStop();

                    timer.Advance(1);

                    ui.VerifySet(it => it.Display = "00:00:01");
                }

                [TestMethod]
                public void UpdatesTheClockEachSecond()
                {
                    sut.StartStop();

                    timer.Advance(3);

                    ui.VerifySet(it => it.Display = "00:00:01");
                    ui.VerifySet(it => it.Display = "00:00:02");
                    ui.VerifySet(it => it.Display = "00:00:03");
                }
            }

            [TestClass]
            public class Stopping : StartStop
            {
                [TestInitialize]
                public void InnerSetUp()
                {
                    sut.Initialize();
                }

                [TestMethod]
                public void UpdatesTheUIState()
                {
                    sut.StartStop();

                    sut.StartStop();

                    ui.VerifySet(it => it.StartStopEnabled = false);
                    ui.VerifySet(it => it.ResetEnabled = true);
                    ui.VerifySet(it => it.HoldEnabled = false);
                }

                [TestMethod]
                public void StopsUpdatingTheClock()
                {
                    sut.StartStop();

                    sut.StartStop();
                    timer.Advance(3);

                    ui.VerifySet(it => it.Display = "00:00:01", Times.Never);
                }
            }
        }

        [TestClass]
        public class Hold : WatchTests
        {
            [TestInitialize]
            public void InnerSetUp()
            {
                sut.Initialize();
            }

            [TestMethod]
            public void FreezesTheScreen()
            {
                sut.StartStop();

                sut.Hold();
                timer.Advance(1);

                ui.VerifySet(it => it.Display = "00:00:01", Times.Never);
            }

            [TestMethod]
            public void UnfreezesTheScreen()
            {
                sut.StartStop();
                sut.Hold();

                sut.Hold();
                timer.Advance(1);

                ui.VerifySet(it => it.Display = "00:00:01");
            }

            [TestMethod]
            public void TheClockContinuesToIncrementEvenWhilePaused()
            {
                sut.StartStop();
                sut.Hold();
                timer.Advance(3);

                sut.Hold();
                timer.Advance(1);

                ui.VerifySet(it => it.Display = "00:00:04");
            }
        }

        [TestClass]
        public class Reset : WatchTests
        {
            [TestInitialize]
            public void InnerSetUp()
            {
                sut.Initialize();
            }

            [TestMethod]
            public void ResetsTheUI()
            {
                sut.StartStop();
                sut.StartStop();

                sut.Reset();

                ui.VerifySet(it => it.StartStopEnabled = true);
                ui.VerifySet(it => it.ResetEnabled = false);
                ui.VerifySet(it => it.HoldEnabled = false);
                ui.VerifySet(it => it.Display = "00:00:00");
            }
        }
    }
}