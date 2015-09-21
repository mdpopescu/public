using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class LocalCheckerTests
  {
    private Mock<LicenseChecker> remote;
    private Mock<Validator> validator;

    private LocalChecker sut;

    [TestInitialize]
    public void SetUp()
    {
      remote = new Mock<LicenseChecker>();
      validator = new Mock<Validator>();

      sut = new LocalChecker(remote.Object, validator.Object);
    }

    [TestClass]
    public class Check : LocalCheckerTests
    {
      // IsLicensed

      [TestMethod]
      public void SetsIsLicensedToFalseIfNotValid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsLicensed);
      }

      [TestMethod]
      public void ChecksRegistrationWithRemoteServerIfValid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

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
      public void SetsIsLicensedToFalseIfServerDeclinesIt()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus {IsLicensed = false});

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsLicensed);
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
          .Returns(new LicenseStatus())
          .Callback<LicenseRegistration>(r => r.Expiration = ObjectMother.OldDate);

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfServerConfirmsAndNotExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus {IsLicensed = true});

        var result = sut.Check(registration);

        Assert.IsTrue(result.IsLicensed);
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
          .Returns(new LicenseStatus());

        var result = sut.Check(registration);

        Assert.IsTrue(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfNull()
      {
        var result = sut.Check(null);

        Assert.IsFalse(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfLimitsIsNull()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits = null;

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfDaysNotNegativeAndExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits.Days = 5;

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfRemainingRunsIsZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 0;

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfRemainingDaysAndRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};

        var result = sut.Check(registration);

        Assert.IsTrue(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfRemainingDaysAndNegativeRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = -1};

        var result = sut.Check(registration);

        Assert.IsTrue(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfNegativeDaysAndRemainingRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits = new Limits {Days = -1, Runs = 1};

        var result = sut.Check(registration);

        Assert.IsTrue(result.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfPastDeadline()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus())
          .Callback<LicenseRegistration>(r => r.Expiration = ObjectMother.OldDate);

        var result = sut.Check(registration);

        Assert.IsFalse(result.IsTrial);
      }
    }
  }
}