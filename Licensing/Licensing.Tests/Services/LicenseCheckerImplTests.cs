using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class LicenseCheckerImplTests
  {
    private Mock<RemoteChecker> remote;
    private Mock<Validator> validator;

    private LicenseChecker sut;

    [TestInitialize]
    public void SetUp()
    {
      remote = new Mock<RemoteChecker>();
      validator = new Mock<Validator>();

      sut = new LicenseCheckerImpl(remote.Object, validator.Object);
    }

    [TestClass]
    public class Check : LicenseCheckerImplTests
    {
      // IsLicensed

      [TestMethod]
      public void SetsIsLicensedToFalseIfNotValid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.Check(registration);

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void ChecksRegistrationWithRemoteServerIfValid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Check(registration);

        remote.Verify(it => it.Check(registration));
      }

      [TestMethod]
      public void DoesNotCheckWithServerIfNull()
      {
        sut.Check(null);

        remote.Verify(it => it.Check(It.IsAny<LicenseRegistration>()), Times.Never);
      }

      [TestMethod]
      public void DoesNotCheckWithServerIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.Check(registration);

        remote.Verify(it => it.Check(It.IsAny<LicenseRegistration>()), Times.Never);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfServerResponseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns((DateTime?) null);

        sut.Check(registration);

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void UpdatesExpirationDateToReturnedValue()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = new DateTime(2000, 1, 2);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(2001, 2, 3));

        sut.Check(registration);

        Assert.AreEqual(new DateTime(2001, 2, 3), registration.Expiration);
      }

      [TestMethod]
      public void DoesNotUpdateExpirationDateIfServerResponseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = new DateTime(2000, 1, 2);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns((DateTime?) null);

        sut.Check(registration);

        Assert.AreEqual(new DateTime(2000, 1, 2), registration.Expiration);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(2001, 2, 3));

        sut.Check(registration);

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfNotExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));

        sut.Check(registration);

        Assert.IsTrue(sut.IsLicensed);
      }

      // IsTrial

      [TestMethod]
      public void SetsIsTrialToTrueIfLicensed()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));

        sut.Check(registration);

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfNull()
      {
        sut.Check(null);

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfLimitsIsNull()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits = null;

        sut.Check(registration);

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfDaysNotNegativeAndExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits.Days = 5;

        sut.Check(registration);

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfRemainingRunsIsZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 0;

        sut.Check(registration);

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfRemainingDaysAndRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};

        sut.Check(registration);

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfRemainingDaysAndNegativeRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = -1};

        sut.Check(registration);

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfNegativeDaysAndRemainingRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits = new Limits {Days = -1, Runs = 1};

        sut.Check(registration);

        Assert.IsTrue(sut.IsTrial);
      }
    }
  }
}