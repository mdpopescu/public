using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;
using TransportTycoon.Library.Services;

namespace TransportTycoon.Tests.Services
{
    [TestClass]
    public class AppTests
    {
        private Mock<IMap> map;

        private App sut;

        [TestInitialize]
        public void SetUp()
        {
            map = new Mock<IMap>();

            sut = new App(map.Object);
        }

        [TestClass]
        public class Run : AppTests
        {
            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void ThrowsAnExceptionIfNoDestinationsAreReceived()
            {
                map.Setup(it => it.Routes).Returns(new[] { new Route("a", "b", 0) });

                sut.Run("");
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidOperationException))]
            public void ThrowsAnExceptionIfNoRoutesAreDefined()
            {
                map.Setup(it => it.Routes).Returns(Enumerable.Empty<Route>());

                sut.Run("A");
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidOperationException))]
            public void ThrowsAnExceptionIfNoRoutesAreFoundForTheGivenDestination()
            {
                map.Setup(it => it.Routes).Returns(new[] { new Route("Factory", "B", 5), });

                sut.Run("A");
            }
        }
    }
}