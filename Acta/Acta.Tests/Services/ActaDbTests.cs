using System;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;
using Acta.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Acta.Tests.Services
{
    [TestClass]
    public class ActaDbTests
    {
        private Mock<ActaStorage> storage;
        private ActaDb sut;

        [TestInitialize]
        public void SetUp()
        {
            storage = new Mock<ActaStorage>();
            sut = new ActaDb(storage.Object);
        }

        [TestClass]
        public class WriteOne : ActaDbTests
        {
            [TestMethod]
            public void WritesTupleToStorage()
            {
                var guid = Guid.NewGuid();

                sut.Write(guid, "test", "value");

                storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "TEST" && (string) t.Value == "value")));
            }
        }

        [TestClass]
        public class WriteMany : ActaDbTests
        {
            [TestMethod]
            public void WritesMultipleTuplesToStorage()
            {
                var guid = Guid.NewGuid();

                sut.Write(guid,
                    new ActaKeyValuePair("k1", "v1"),
                    new ActaKeyValuePair("k2", "v2"));

                storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "K1" && (string) t.Value == "v1")));
                storage.Verify(it => it.Append(It.Is<ActaTuple>(t => t.Id == guid && t.Name == "K2" && (string) t.Value == "v2")));
            }
        }

        [TestClass]
        public class GetIds : ActaDbTests
        {
            [TestMethod]
            public void ReturnsTheGuidForTheMatchingEntity()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.Get())
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value", 0),
                    });

                var result = sut.GetIds("test", "value").ToList();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(guid, result[0]);
            }

            [TestMethod]
            public void ReturnsGuidsForAllMatchingEntities()
            {
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                storage
                    .Setup(it => it.Get())
                    .Returns(new[]
                    {
                        new ActaTuple(guid1, "test", "value", 0),
                        new ActaTuple(guid2, "test", "value", 0),
                    });

                var result = sut.GetIds("test", "value").ToArray();

                CollectionAssert.AreEquivalent(new[] { guid1, guid2 }, result);
            }

            [TestMethod]
            public void DoesNotReturnTheGuidsOfNonMatchingEntities()
            {
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                var guid3 = Guid.NewGuid();
                storage
                    .Setup(it => it.Get())
                    .Returns(new[]
                    {
                        new ActaTuple(guid1, "test", "value", 0),
                        new ActaTuple(guid2, "test", "wrong value", 0),
                        new ActaTuple(guid3, "test", "value", 0),
                    });

                var result = sut.GetIds("test", "value").ToArray();

                CollectionAssert.AreEquivalent(new[] { guid1, guid3 }, result);
            }

            [TestMethod]
            public void DoesNotReturnDuplicateGuids()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.Get())
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value", 0),
                        new ActaTuple(guid, "test", "value", 0),
                    });

                var result = sut.GetIds("test", "value").ToList();

                Assert.AreEqual(1, result.Count);
            }
        }

        [TestClass]
        public class Read : ActaDbTests
        {
            [TestMethod]
            public void CallsStorageGetById()
            {
                var guid = Guid.NewGuid();

                sut.Read(guid, "test");

                storage.Verify(it => it.GetById(guid));
            }

            [TestMethod]
            public void ReturnsMatchingValue()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value", 0),
                    });

                var result = sut.Read(guid, "test") as string;

                Assert.AreEqual("value", result);
            }

            [TestMethod]
            public void ReturnsNullIfPropertyNotFoundForTheGivenId()
            {
                var result = sut.Read(Guid.NewGuid(), "test");

                Assert.IsNull(result);
            }

            [TestMethod]
            public void ReturnsTheMostRecentValue()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value1", 0),
                        new ActaTuple(guid, "test", "value2", 1),
                    });

                var result = sut.Read(guid, "test") as string;

                Assert.AreEqual("value2", result);
            }

            [TestMethod]
            public void TheGenericVersionConvertsTheResult()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value", 0),
                    });

                var result = sut.Read<string>(guid, "test");

                Assert.AreEqual("value", result);
            }

            [TestMethod]
            public void TheGenericVersionReturnsTheDefaultValueIfTheResultDoesntExist()
            {
                var result = sut.Read<int>(Guid.NewGuid(), "test");

                Assert.AreEqual(0, result);
            }

            [TestMethod]
            public void ReturnsAnEmptyListIfNoPropertyFoundForGivenId()
            {
                var result = sut.Read(Guid.NewGuid()).ToList();

                Assert.AreEqual(0, result.Count);
            }

            [TestMethod]
            public void ReturnsTheListOfKeyValuePairsForGivenId()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test1", "value1", 0),
                        new ActaTuple(guid, "test2", "value2", 0),
                    });

                var result = sut.Read(guid).ToList();

                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("TEST1", result[0].Name);
                Assert.AreEqual("value1", result[0].Value);
                Assert.AreEqual("TEST2", result[1].Name);
                Assert.AreEqual("value2", result[1].Value);
            }

            [TestMethod]
            public void ReturnsTheMostRecentValue_2()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value1", 0),
                        new ActaTuple(guid, "test", "value2", 1),
                    });

                var result = sut.Read(guid).ToList();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("value2", result[0].Value);
            }

            [TestMethod]
            public void TheNameCapitalizationIsIgnored()
            {
                var guid = Guid.NewGuid();
                storage
                    .Setup(it => it.GetById(guid))
                    .Returns(new[]
                    {
                        new ActaTuple(guid, "test", "value1", 0),
                        new ActaTuple(guid, "TEST", "value2", 1),
                    });

                var result = sut.Read(guid).ToList();

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("value2", result[0].Value);
            }
        }
    }
}