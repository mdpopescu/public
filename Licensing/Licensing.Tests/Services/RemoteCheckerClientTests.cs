using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class RemoteCheckerClientTests
  {
    private Mock<Sys> sys;
    private Mock<Remote> remote;
    private Mock<ResponseParser> parser;

    private RemoteCheckerClient sut;

    [TestInitialize]
    public void SetUp()
    {
      sys = new Mock<Sys>();
      remote = new Mock<Remote>();
      parser = new Mock<ResponseParser>();

      sut = new RemoteCheckerClient(sys.Object, remote.Object, parser.Object);
    }

    [TestClass]
    public class Check : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Check(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void OverwritesTheProcessorIdInTheRegistration()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("5");

        sut.Check(registration);

        Assert.AreEqual("5", registration.ProcessorId);
      }

      [TestMethod]
      public void SendsKeyToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Check(registration);

        remote.Verify(it => it.Get(It.Is<string>(s => s.StartsWith("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}"))));
      }

      [TestMethod]
      public void SendsProcessorIdToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.Check(registration);

        remote.Verify(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"));
      }

      [TestMethod]
      public void ParsesTheResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");

        sut.Check(registration);

        parser.Verify(it => it.Parse("abc"));
      }

      [TestMethod]
      public void ReturnsNullIfTheKeyDoesNotMatch()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "def"});

        var result = sut.Check(registration);

        Assert.IsNull(result);
      }

      [TestMethod]
      public void ReturnsTheExpirationDateIfTheKeyMatches()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "{D98F6376-94F7-4D82-AA37-FC00F0166700}", Expiration = new DateTime(2000, 1, 2)});

        var result = sut.Check(registration);

        Assert.AreEqual(new DateTime(2000, 1, 2), result);
      }
    }

    [TestClass]
    public class Submit : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Submit(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void OverwritesTheProcessorIdInTheRegistration()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("5");

        sut.Submit(registration);

        Assert.AreEqual("5", registration.ProcessorId);
      }

      [TestMethod]
      public void EncodesTheRegistrationDetails()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.Submit(registration);

        sys.Verify(it => it.Encode(
          It.Is<IEnumerable<KeyValuePair<string, string>>>(
            e => e.Any(pair => pair.Value == "{D98F6376-94F7-4D82-AA37-FC00F0166700}"))));
      }

      [TestMethod]
      public void SendsTheRegistrationDetailsToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");

        sut.Submit(registration);

        remote.Verify(it => it.Post("abc"));
      }

      [TestMethod]
      public void ParsesTheResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Post(It.IsAny<string>()))
          .Returns("abc");

        sut.Submit(registration);

        parser.Verify(it => it.Parse("abc"));
      }

      [TestMethod]
      public void ReturnsNullIfTheKeyDoesNotMatch()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Post(It.IsAny<string>()))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "def"});

        var result = sut.Submit(registration);

        Assert.IsNull(result);
      }

      [TestMethod]
      public void ReturnsTheExpirationDateIfTheKeyMatches()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Post(It.IsAny<string>()))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "{D98F6376-94F7-4D82-AA37-FC00F0166700}", Expiration = new DateTime(2000, 1, 2)});

        var result = sut.Submit(registration);

        Assert.AreEqual(new DateTime(2000, 1, 2), result);
      }
    }
  }
}