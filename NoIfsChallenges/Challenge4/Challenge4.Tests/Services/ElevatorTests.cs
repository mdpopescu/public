using Challenge4.Library.Models;
using Challenge4.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge4.Tests.Services
{
    [TestClass]
    public class ElevatorTests
    {
        private Elevator sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new Elevator();
        }

        [TestClass]
        public class InitialState : ElevatorTests
        {
            [TestMethod]
            public void ReturnsTheInitialState()
            {
                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, false, true, true, "Door Open");
            }
        }

        [TestClass]
        public class Scenario_1stFloor : ElevatorTests
        {
            [TestInitialize]
            public void InnerSetUp()
            {
                // the elevator starts on the 1st floor
            }

            [TestMethod]
            public void GoTo2ndFloor()
            {
                sut.GoTo2nd();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, false, true, true, "Elevator Arrives - Door Opens - User Exists");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                sut.GoTo3rd();

                CheckFloor(sut.Info.Floor3, false, true, true, "Elevator Arrives - Door Opens - User Exists");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                sut.CallTo2nd();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, false, true, true, "Elevator Called Up - Door Opens");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                sut.CallTo3rd();

                CheckFloor(sut.Info.Floor3, false, true, true, "Elevator Called Up - Door Opens");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }
        }

        [TestClass]
        public class Scenario_2ndFloor : ElevatorTests
        {
            [TestInitialize]
            public void InnerSetUp()
            {
                sut.CallTo2nd();
            }

            [TestMethod]
            public void GoTo1stFloor()
            {
                sut.GoTo1st();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, false, true, true, "Elevator Arrives - Door Opens - User Exists");
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                sut.GoTo3rd();

                CheckFloor(sut.Info.Floor3, false, true, true, "Elevator Arrives - Door Opens - User Exists");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                sut.CallTo1st();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, false, true, true, "Elevator Called Up - Door Opens");
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                sut.CallTo3rd();

                CheckFloor(sut.Info.Floor3, false, true, true, "Elevator Called Up - Door Opens");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }
        }

        [TestClass]
        public class Scenario_3rdFloor : ElevatorTests
        {
            [TestInitialize]
            public void InnerSetUp()
            {
                sut.CallTo3rd();
            }

            [TestMethod]
            public void GoTo1stFloor()
            {
                sut.GoTo1st();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, false, true, true, "Elevator Arrives - Door Opens - User Exists");
            }

            [TestMethod]
            public void GoTo2ndFloor()
            {
                sut.GoTo2nd();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, false, true, true, "Elevator Arrives - Door Opens - User Exists");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                sut.CallTo1st();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor1, false, true, true, "Elevator Called Up - Door Opens");
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                sut.CallTo2nd();

                CheckFloor(sut.Info.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.Info.Floor2, false, true, true, "Elevator Called Up - Door Opens");
                CheckFloor(sut.Info.Floor1, true, false, false, "Door Closed");
            }
        }

        //

        private static void CheckFloor(FloorInfo floorInfo, bool button1, bool button2, bool button3, string screen)
        {
            Assert.AreEqual(button1, floorInfo.Button1Enabled);
            Assert.AreEqual(button2, floorInfo.Button2Enabled);
            Assert.AreEqual(button3, floorInfo.Button3Enabled);
            Assert.AreEqual(screen, floorInfo.Screen);
        }
    }
}