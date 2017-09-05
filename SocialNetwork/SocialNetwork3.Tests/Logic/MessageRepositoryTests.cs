using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Logic;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Tests.Logic
{
    [TestClass]
    public class MessageRepositoryTests
    {
        private MessageRepository sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MessageRepository();
        }

        [TestMethod]
        public void AddedMessagesGetReturned()
        {
            var dt = new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Add(new Message(dt, "a", "1"));
            sut.Add(new Message(dt, "a", "2"));

            var result = sut.GetMessagesBy("a").ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result[0].Text);
            Assert.AreEqual("2", result[1].Text);
        }

        [TestMethod]
        public void DoesNotReturnMessagesAddedByADifferentUser()
        {
            var dt = new DateTime(2000, 1, 2, 3, 4, 5);
            sut.Add(new Message(dt, "a", "1"));
            sut.Add(new Message(dt, "b", "2"));

            var result = sut.GetMessagesBy("b").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("2", result[0].Text);
        }

        [TestMethod]
        public void SortsTheMessagesByTimeInDescendingOrder()
        {
            sut.Add(new Message(new DateTime(2000, 1, 2, 3, 4, 5), "a", "1"));
            sut.Add(new Message(new DateTime(2000, 1, 2, 3, 4, 6), "a", "2"));

            var result = sut.GetMessagesBy("a").ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("2", result[0].Text);
            Assert.AreEqual("1", result[1].Text);
        }
    }
}