using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;
using TransportTycoon.Library.Services;

namespace TransportTycoon.Tests
{
    [TestClass]
    [TestCategory("End to End")]
    public class EndToEndTests
    {
        private App app;

        [TestInitialize]
        public void SetUp()
        {
            var endpoints = new[]
            {
                new Endpoint("Factory"),
                new Endpoint("Port"),
                new Endpoint("A"),
                new Endpoint("B"),
            };

            IMap map = new Map();
            map.AddEndpoints(endpoints);

            IVehicle truck = new Vehicle("Truck");
            truck.AddRoute(endpoints[0], endpoints[1], 1);
            truck.AddRoute(endpoints[0], endpoints[3], 5);

            IVehicle boat = new Vehicle("Boat");
            boat.AddRoute(endpoints[1], endpoints[2], 4);

            app = new App(map);
            app.AddVehicle(truck, "t1");
            app.AddVehicle(truck, "t2");
            app.AddVehicle(boat, "b");
        }

        [TestClass]
        public class KnownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(5, app.Run("A"));
            }

            [TestMethod]
            public void Test2()
            {
                Assert.AreEqual(5, app.Run("AB"));
            }

            [TestMethod]
            public void Test3()
            {
                Assert.AreEqual(5, app.Run("BB"));
            }

            [TestMethod]
            public void Test4()
            {
                Assert.AreEqual(7, app.Run("ABB"));
            }
        }

        [TestClass]
        public class UnknownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(0, app.Run("AABABBAB"));
            }
        }
    }
}