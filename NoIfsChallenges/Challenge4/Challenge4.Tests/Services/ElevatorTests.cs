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

                Assert.IsTrue(sut.ElevatorInfo.Floor3.Button1Enabled);
                Assert.IsFalse(sut.ElevatorInfo.Floor3.Button2Enabled);
                Assert.IsFalse(sut.ElevatorInfo.Floor3.Button3Enabled);
                Assert.AreEqual("Door Closed", sut.ElevatorInfo.Floor3.Screen);

                Assert.IsTrue(sut.ElevatorInfo.Floor2.Button1Enabled);
                Assert.IsFalse(sut.ElevatorInfo.Floor2.Button2Enabled);
                Assert.IsFalse(sut.ElevatorInfo.Floor2.Button3Enabled);
                Assert.AreEqual("Door Closed", sut.ElevatorInfo.Floor2.Screen);

                Assert.IsFalse(sut.ElevatorInfo.Floor1.Button1Enabled);
                Assert.IsTrue(sut.ElevatorInfo.Floor1.Button2Enabled);
                Assert.IsTrue(sut.ElevatorInfo.Floor1.Button3Enabled);
                Assert.AreEqual("Door Open", sut.ElevatorInfo.Floor1.Screen);
            }
        }
    }
}