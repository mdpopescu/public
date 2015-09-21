using System;
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
    private Mock<Remote> remote;
    private Mock<RequestBuilder> builder;
    private Mock<ResponseParser> parser;

    private RemoteCheckerClient sut;

    [TestInitialize]
    public void SetUp()
    {
      remote = new Mock<Remote>();
      builder = new Mock<RequestBuilder>();
      parser = new Mock<ResponseParser>();

      sut = new RemoteCheckerClient(remote.Object, builder.Object, parser.Object);
    }

    [TestClass]
    public class Check : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheQuery()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Check(registration);

        builder.Verify(it => it.BuildQuery(registration));
      }

      [TestMethod]
      public void SendsQueryToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");

        sut.Check(registration);

        remote.Verify(it => it.Get("query"));
      }

      [TestMethod]
      public void ParsesTheResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");

        sut.Check(registration);

        parser.Verify(it => it.Parse("abc"));
      }

      [TestMethod]
      public void UpdatesExpirationDateToReturnedValue()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = ObjectMother.OldDate;
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = ObjectMother.KEY, Expiration = ObjectMother.NewDate});

        sut.Check(registration);

        Assert.AreEqual(ObjectMother.NewDate, registration.Expiration);
      }

      [TestMethod]
      public void DoesNotUpdateExpirationDateToMinimumValueIfResponseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = ObjectMother.OldDate;
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "def"});

        sut.Check(registration);

        Assert.AreEqual(ObjectMother.OldDate, registration.Expiration);
      }

      [TestMethod]
      public void UpdatesCreatedDateToTodayWhenResponseIsValid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = ObjectMother.OldDate;
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = ObjectMother.KEY, Expiration = ObjectMother.NewDate});

        sut.Check(registration);

        Assert.AreEqual(DateTime.Today, registration.CreatedOn);
      }

      [TestMethod]
      public void UpdatesNumberOfDaysTo30WhenResponseIsValid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Days = 1;
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse { Key = ObjectMother.KEY, Expiration = ObjectMother.NewDate });

        sut.Check(registration);

        Assert.AreEqual(30, registration.Limits.Days);
      }

      [TestMethod]
      public void UpdatesNumberOfRunsToMinusOneWhenResponseIsValid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 1;
        builder
          .Setup(it => it.BuildQuery(registration))
          .Returns("query");
        remote
          .Setup(it => it.Get("query"))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse { Key = ObjectMother.KEY, Expiration = ObjectMother.NewDate });

        sut.Check(registration);

        Assert.AreEqual(-1, registration.Limits.Runs);
      }
    }

    [TestClass]
    public class Submit : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheData()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Submit(registration);

        builder.Verify(it => it.BuildData(registration));
      }

      [TestMethod]
      public void SendsTheDataToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        builder
          .Setup(it => it.BuildData(registration))
          .Returns("data");

        sut.Submit(registration);

        remote.Verify(it => it.Post("data"));
      }
    }
  }
}