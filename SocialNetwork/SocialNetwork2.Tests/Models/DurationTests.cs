using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Tests.Models
{
    [TestClass]
    public class DurationTests
    {
        [TestMethod]
        public void ReturnsElapsedSeconds()
        {
            var duration = new Duration(TimeSpan.FromSeconds(2));

            Assert.AreEqual("2 seconds", duration.ToString());
        }

        [TestMethod]
        public void ReturnsElapsedSecond()
        {
            var duration = new Duration(TimeSpan.FromSeconds(1));

            Assert.AreEqual("1 second", duration.ToString());
        }

        [TestMethod]
        public void ReturnsElapsedMinutes()
        {
            var duration = new Duration(TimeSpan.FromMinutes(2));

            Assert.AreEqual("2 minutes", duration.ToString());
        }

        [TestMethod]
        public void ReturnsElapsedMinute()
        {
            var duration = new Duration(TimeSpan.FromMinutes(1));

            Assert.AreEqual("1 minute", duration.ToString());
        }

        [TestMethod]
        public void ReturnsElapsedHours()
        {
            var duration = new Duration(TimeSpan.FromHours(2));

            Assert.AreEqual("2 hours", duration.ToString());
        }

        [TestMethod]
        public void ReturnsElapsedHour()
        {
            var duration = new Duration(TimeSpan.FromHours(1));

            Assert.AreEqual("1 hour", duration.ToString());
        }
    }
}