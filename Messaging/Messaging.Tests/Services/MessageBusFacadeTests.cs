using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using AutoBogus;
using Messaging.Library.Contracts;
using Messaging.Library.Models;
using Messaging.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Messaging.Tests.Services
{
    [TestClass]
    public class MessageBusFacadeTests
    {
        private readonly Mock<IMessageSerializer<string>> serializer = new();
        private readonly Mock<IPubSub<string>> comms = new();

        private readonly MessageBusFacade<string> sut;

        public MessageBusFacadeTests()
        {
            sut = new MessageBusFacade<string>(serializer.Object, comms.Object);
        }

        [TestClass]
        public class Publish : MessageBusFacadeTests
        {
            [TestMethod("1. Serializes the message")]
            public void Test1()
            {
                var message = new TestMessage(Guid.NewGuid(), null, null, 0);

                sut.Publish(message);

                serializer.Verify(it => it.Serialize(message));
            }

            [TestMethod("2. Publishes the serialized message")]
            public void Test2()
            {
                var message = new TestMessage(Guid.NewGuid(), null, null, 0);
                var serialized = AutoFaker.Generate<string>();
                serializer.Setup(it => it.Serialize(message)).Returns(serialized);

                sut.Publish(message);

                comms.Verify(it => it.Publish(serialized));
            }
        }

        [TestClass]
        public class GetMessages : MessageBusFacadeTests
        {
            private readonly Subject<string> bus = new();
            private readonly List<IMessage> incoming = new();

            public GetMessages()
            {
                comms.Setup(it => it.Messages).Returns(bus);
            }

            [TestMethod("1. Detects incoming messages")]
            public void Test1()
            {
                using var subscription = sut.GetMessages().Subscribe(incoming.Add);
                var message = new TestMessage(Guid.NewGuid(), null, null, 0);
                serializer.Setup(it => it.Deserialize(It.IsAny<string>())).Returns(message);

                bus.OnNext(AutoFaker.Generate<string>());

                Assert.AreEqual(1, incoming.Count);
            }

            [TestMethod("2. Deserializes the incoming messages")]
            public void Test2()
            {
                using var subscription = sut.GetMessages().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                var message = new TestMessage(Guid.NewGuid(), null, null, 0);
                serializer.Setup(it => it.Deserialize(serialized)).Returns(message);

                bus.OnNext(serialized);

                Assert.AreEqual(1, incoming.Count);
                Assert.AreEqual(message, incoming[0]);
            }

            [TestMethod("3A. Ignores serialization failures - throwing")]
            public void Test3A()
            {
                using var subscription = sut.GetMessages().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                serializer.Setup(it => it.Deserialize(serialized)).Throws<Exception>();

                bus.OnNext(serialized);

                Assert.AreEqual(0, incoming.Count);
            }

            [TestMethod("3B. Ignores serialization failures - null")]
            public void Test3B()
            {
                using var subscription = sut.GetMessages().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                serializer.Setup(it => it.Deserialize(serialized)).Returns((IMessage)null!);

                bus.OnNext(serialized);

                Assert.AreEqual(0, incoming.Count);
            }
        }

        [TestClass]
        public class GetErrors : MessageBusFacadeTests
        {
            private readonly Subject<string> bus = new();
            private readonly List<IErrorMessage> incoming = new();

            public GetErrors()
            {
                comms.Setup(it => it.Messages).Returns(bus);
            }

            [TestMethod("1. Returns serialization failures - throwing")]
            public void Test1()
            {
                using var subscription = sut.GetErrors().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                serializer.Setup(it => it.Deserialize(serialized)).Throws<Exception>();

                bus.OnNext(serialized);

                Assert.AreEqual(1, incoming.Count);
                Assert.AreEqual("Deserialization error", incoming[0].Description);
                Assert.AreEqual(serialized, incoming[0].AdditionalInfo);
            }

            [TestMethod("2. Returns serialization failures - null")]
            public void Test2()
            {
                using var subscription = sut.GetErrors().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                serializer.Setup(it => it.Deserialize(serialized)).Returns((IMessage)null!);

                bus.OnNext(serialized);

                Assert.AreEqual(1, incoming.Count);
                Assert.AreEqual("Deserialization error", incoming[0].Description);
                Assert.AreEqual(serialized, incoming[0].AdditionalInfo);
            }

            [TestMethod("3. Returns incoming errors")]
            public void Test3()
            {
                using var subscription = sut.GetErrors().Subscribe(incoming.Add);
                var serialized = AutoFaker.Generate<string>();
                var description = AutoFaker.Generate<string>();
                var additionalInfo = AutoFaker.Generate<string>();
                var message = new ErrorMessage(Guid.NewGuid(), null, null, 0, description, additionalInfo);
                serializer.Setup(it => it.Deserialize(serialized)).Returns(message);

                bus.OnNext(serialized);

                Assert.AreEqual(1, incoming.Count);
                Assert.AreEqual(description, incoming[0].Description);
                Assert.AreEqual(additionalInfo, incoming[0].AdditionalInfo);
            }
        }

        //

        private class TestMessage : MessageBase
        {
            public TestMessage(Guid id, Guid? categoryId, Guid? inReplyTo, int version)
                : base(id, categoryId, inReplyTo, version)
            {
            }
        }
    }
}