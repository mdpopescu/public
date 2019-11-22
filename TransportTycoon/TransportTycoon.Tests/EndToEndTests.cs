using System.Linq;
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

            var links = new[]
            {
                new Link(endpoints[0], endpoints[1]),
                new Link(endpoints[0], endpoints[3]),
                new Link(endpoints[1], endpoints[2]),
            };

            // I am not sure that the map is useful
            IMap map = new Map(links);

            IVehicle truck = new Vehicle("Truck");
            truck.SetCost(links[0], 1);
            truck.SetCost(links[1], 5);

            IVehicle boat = new Vehicle("Boat");
            boat.SetCost(links[2], 4);

            app = new App(map);
            app.AddVehicle(truck, "t1", endpoints[0]);
            app.AddVehicle(truck, "t2", endpoints[0]);
            app.AddVehicle(boat, "b", endpoints[1]);
        }

        [TestClass]
        public class KnownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(5, app.Run("Factory", AsStrings("B")));
            }

            [TestMethod]
            public void Test2()
            {
                Assert.AreEqual(5, app.Run("Factory", AsStrings("A")));
            }

            [TestMethod]
            public void Test3()
            {
                Assert.AreEqual(5, app.Run("Factory", AsStrings("AB")));
            }

            [TestMethod]
            public void Test4()
            {
                Assert.AreEqual(5, app.Run("Factory", AsStrings("BB")));
            }

            [TestMethod]
            public void Test5()
            {
                Assert.AreEqual(7, app.Run("Factory", AsStrings("ABB")));
            }
        }

        [TestClass]
        public class UnknownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(0, app.Run("Factory", AsStrings("AABABBAB")));
            }
        }

        //

        private static string[] AsStrings(string str)
        {
            return str.Select(it => it.ToString()).ToArray();
        }
    }
}