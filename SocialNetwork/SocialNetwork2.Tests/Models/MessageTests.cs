using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork2.Library.Implementations;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Tests.Models
{
    [TestClass]
    public class MessageTests
    {
        [TestClass]
        // ReSharper disable once InconsistentNaming
        public class _ToString : MessageTests
        {
            [TestMethod]
            public void ReturnsMessagePlusElapsedTimeInSeconds()
            {
                Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
                var sut = new Message("user", "abc");

                Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 7);
                var result = sut.ToString();

                Assert.AreEqual("abc (2 seconds ago)", result);
            }
        }

        [TestClass]
        // ReSharper disable once InconsistentNaming
        public class _ToTaggedString : MessageTests
        {
            [TestMethod]
            public void ReturnsUserNamePlusMessagePlusElapsedTimeInSeconds()
            {
                Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 5);
                var sut = new Message("user", "abc");

                Sys.Time = () => new DateTime(2000, 1, 2, 3, 4, 7);
                var result = sut.ToTaggedString();

                Assert.AreEqual("user - abc (2 seconds ago)", result);
            }
        }
    }
}