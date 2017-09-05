using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Logic;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Tests.Logic
{
    [TestClass]
    public class MessageFormatterTests
    {
        private MessageFormatter sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MessageFormatter();
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeOfOneSecond()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 3, 4, 6));

            Assert.AreEqual("b (1 second ago)", result);
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeInSeconds()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 3, 4, 7));

            Assert.AreEqual("b (2 seconds ago)", result);
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeOfOneMinute()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 3, 5, 5));

            Assert.AreEqual("b (1 minute ago)", result);
        }

        [TestMethod]
        public void ReturnsMessagePlusElapsedTimeInMinutes()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 3, 6, 5));

            Assert.AreEqual("b (2 minutes ago)", result);
        }

        [TestMethod]
        public void TimeInMinutesIsRoundedDown()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 3, 6, 55));

            Assert.AreEqual("b (2 minutes ago)", result);
        }

        [TestMethod]
        public void ReturnsMessageWhenElapsedTimeIsAnHourOrMore()
        {
            var msg = new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "b");

            var result = sut.Format(msg, new DateTime(2000, 1, 2, 5, 4, 5));

            Assert.AreEqual("b (long ago)", result);
        }
    }
}