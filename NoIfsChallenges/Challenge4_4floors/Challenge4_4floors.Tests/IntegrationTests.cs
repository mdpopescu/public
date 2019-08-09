using Challenge4_4floors.Library;
using Challenge4_4floors.Library.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge4_4floors.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private Building building;
        private Elevator elevator;

        [TestInitialize]
        public void SetUp()
        {
            building = new Building();
            elevator = new Elevator();
        }

        [TestMethod]
        public void Starting()
        {
            Assert.IsTrue(building.GetFloor(1).HasElevator);
            Assert.IsFalse(building.GetFloor(2).HasElevator);
            Assert.IsFalse(building.GetFloor(3).HasElevator);
            Assert.IsFalse(building.GetFloor(4).HasElevator);

            Assert.AreEqual(Constants.DOOR_CLOSED, building.GetFloor(1).Display);
            Assert.AreEqual(Constants.DOOR_CLOSED, building.GetFloor(2).Display);
            Assert.AreEqual(Constants.DOOR_CLOSED, building.GetFloor(3).Display);
            Assert.AreEqual(Constants.DOOR_CLOSED, building.GetFloor(4).Display);

            Assert.IsFalse(elevator.IsLit1);
            Assert.IsFalse(elevator.IsLit2);
            Assert.IsFalse(elevator.IsLit3);
            Assert.IsFalse(elevator.IsLit4);
            Assert.AreEqual(Constants.DOOR_CLOSED, elevator.Display);
        }
    }
}