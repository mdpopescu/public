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
                var result = sut.Initialize();

                Assert.IsTrue(result.Floor3.Button1Enabled);
                Assert.IsFalse(result.Floor3.Button2Enabled);
                Assert.IsFalse(result.Floor3.Button3Enabled);
                Assert.AreEqual("Door Closed", result.Floor3.Screen);

                Assert.IsTrue(result.Floor2.Button1Enabled);
                Assert.IsFalse(result.Floor2.Button2Enabled);
                Assert.IsFalse(result.Floor2.Button3Enabled);
                Assert.AreEqual("Door Closed", result.Floor2.Screen);

                Assert.IsFalse(result.Floor1.Button1Enabled);
                Assert.IsTrue(result.Floor1.Button2Enabled);
                Assert.IsTrue(result.Floor1.Button3Enabled);
                Assert.AreEqual("Door Open", result.Floor1.Screen);
            }
        }
    }
}