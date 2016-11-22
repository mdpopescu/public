using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Elomen.Library.Contracts;
using Elomen.Library.Model;
using Elomen.TwitterLibrary.Implementations;
using Elomen.TwitterLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Elomen.Tests.Implementations
{
    [TestClass]
    public class TwitterChannelAdapterTests
    {
        private Mock<Channel<TwitterMessage>> channel;
        private Mock<Repository<long, TwitterAccount>> repository;

        private TwitterChannelAdapter sut;

        [TestInitialize]
        public void SetUp()
        {
            channel = new Mock<Channel<TwitterMessage>>();
            repository = new Mock<Repository<long, TwitterAccount>>();

            sut = new TwitterChannelAdapter(channel.Object, repository.Object);
        }

        [TestClass]
        public class Send : TwitterChannelAdapterTests
        {
            [TestMethod]
            public void SendsToTheUnderlyingChannel()
            {
                repository
                    .Setup(it => it.Find(1))
                    .Returns(new TwitterAccount(2, "b"));

                sut.Send(new Message { Account = new Account(1, "a"), Text = "abc" });

                channel.Verify(it => it.Send(It.Is<TwitterMessage>(m => m.Account.Id == 2 &&
                                                                        m.Account.Username == "b" &&
                                                                        m.Text == "abc")));
            }
        }

        [TestClass]
        public class Receive : TwitterChannelAdapterTests
        {
            [TestMethod]
            public void ConvertsTheUnderlyingChannelMessages()
            {
                using (var messages = new Subject<TwitterMessage>())
                {
                    channel
                        .Setup(it => it.Receive())
                        .Returns(messages);
                    var list = new List<Message>();
                    sut.Receive().Subscribe(list.Add);

                    messages.OnNext(new TwitterMessage(new TwitterAccount(1, "a"), "b"));

                    Assert.AreEqual(1, list.Count);
                    Assert.Fail("I don't know how to test the account");
                    Assert.AreEqual("b", list[0].Text);
                }
            }
        }
    }
}