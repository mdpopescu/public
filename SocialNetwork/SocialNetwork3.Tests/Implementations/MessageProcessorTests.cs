using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork3.Library.Implementations;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Tests.Implementations
{
    [TestClass]
    public class MessageProcessorTests
    {
        private MessageProcessor sut;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MessageProcessor();
        }

        [TestMethod]
        public void PostAddsTheMessage()
        {
            var list = new List<Message>();
            var time = new DateTime(2000, 1, 2, 3, 4, 5);

            sut.ProcessLine("user -> message", time, list.Add);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(time, list[0].Time);
            Assert.AreEqual("user", list[0].User);
            Assert.AreEqual("message", list[0].Text);
        }
    }
}