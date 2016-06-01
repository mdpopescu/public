using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Tests.Models
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeInSeconds()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            var sut = new Message("abc");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 7);
            var result = sut.ToString();

            Assert.AreEqual("abc (2 seconds ago)", result);
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeInSecondsAsSingular()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            var sut = new Message("abc");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 6);
            var result = sut.ToString();

            Assert.AreEqual("abc (1 second ago)", result);
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeInMinutes()
        {
            Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
            var sut = new Message("abc");

            Sys.Time = () => new DateTime(2000, 1, 2, 3, 6, 5);
            var result = sut.ToString();

            Assert.AreEqual("abc (2 minutes ago)", result);
        }
    }
}