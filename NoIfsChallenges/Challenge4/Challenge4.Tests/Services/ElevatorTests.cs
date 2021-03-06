﻿using Challenge4.Library;
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
                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, false, Constants.DOOR_OPEN);
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

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, false, Constants.ELEVATOR_ARRIVES);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                sut.GoTo3rd();

                CheckFloor(sut.Info.Floor3, false, Constants.ELEVATOR_ARRIVES);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                sut.CallTo2nd();

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, false, Constants.ELEVATOR_CALLED_UP);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                sut.CallTo3rd();

                CheckFloor(sut.Info.Floor3, false, Constants.ELEVATOR_CALLED_UP);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
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

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, false, Constants.ELEVATOR_ARRIVES);
            }

            [TestMethod]
            public void GoTo3rdFloor()
            {
                sut.GoTo3rd();

                CheckFloor(sut.Info.Floor3, false, Constants.ELEVATOR_ARRIVES);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                sut.CallTo1st();

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, false, Constants.ELEVATOR_CALLED_DOWN);
            }

            [TestMethod]
            public void CalledTo3rdFloor()
            {
                sut.CallTo3rd();

                CheckFloor(sut.Info.Floor3, false, Constants.ELEVATOR_CALLED_UP);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
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

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, false, Constants.ELEVATOR_ARRIVES);
            }

            [TestMethod]
            public void GoTo2ndFloor()
            {
                sut.GoTo2nd();

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, false, Constants.ELEVATOR_ARRIVES);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }

            [TestMethod]
            public void CalledTo1stFloor()
            {
                sut.CallTo1st();

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor1, false, Constants.ELEVATOR_CALLED_DOWN);
            }

            [TestMethod]
            public void CalledTo2ndFloor()
            {
                sut.CallTo2nd();

                CheckFloor(sut.Info.Floor3, true, Constants.DOOR_CLOSED);
                CheckFloor(sut.Info.Floor2, false, Constants.ELEVATOR_CALLED_DOWN);
                CheckFloor(sut.Info.Floor1, true, Constants.DOOR_CLOSED);
            }
        }

        //

        private static void CheckFloor(FloorInfo floorInfo, bool callEnabled, string screen)
        {
            Assert.AreEqual(callEnabled, floorInfo.CallEnabled);
            Assert.AreEqual(screen, floorInfo.Screen);
        }
    }
}