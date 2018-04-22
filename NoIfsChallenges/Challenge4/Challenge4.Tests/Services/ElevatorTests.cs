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
        public class Initialize : ElevatorTests
        {
            [TestMethod]
            public void ReturnsTheInitialState()
            {
                sut.Initialize();

                CheckFloor(sut.ElevatorInfo.Floor3, true, false, false, "Door Closed");
                CheckFloor(sut.ElevatorInfo.Floor2, true, false, false, "Door Closed");
                CheckFloor(sut.ElevatorInfo.Floor1, false, true, true, "Door Open");
            }
        }

        [TestClass]
        public class Scenario_1stFloor : ElevatorTests
        {
            [TestMethod]
            public void GoTo2ndFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                Assert.Fail();
            }
        }

        [TestClass]
        public class Scenario_2ndFloor : ElevatorTests
        {
            [TestMethod]
            public void GoTo1stFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                Assert.Fail();
            }
        }

        [TestClass]
        public class Scenario_3rdFloor : ElevatorTests
        {
            [TestMethod]
            public void GoTo1stFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void GoTo2ndFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                Assert.Fail();
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