using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class LicenserTests
  {
    private Mock<Storage> storage;
    private Mock<Sys> sys;
    private Mock<RemoteChecker> checker;
    private Mock<Validator> validator;

    private TestLicenser sut;

    [TestInitialize]
    public void SetUp()
    {
      storage = new Mock<Storage>();
      sys = new Mock<Sys>();
      checker = new Mock<RemoteChecker>();
      validator = new Mock<Validator>();

      sut = new TestLicenser(storage.Object, sys.Object, checker.Object, validator.Object);
    }

    [TestClass]
    public class LoadRegistration : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        sut.LoadRegistration();

        storage.Verify(it => it.Load());
      }

      [TestMethod]
      public void SavesANewRegistrationIfNoneExists()
      {
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.LoadRegistration();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r =>
          r.CreatedOn == DateTime.Today &&
          r.Limits.Days == Constants.DEFAULT_DAYS &&
          r.Limits.Runs == Constants.DEFAULT_RUNS &&
          r.Key == null &&
          r.Name == null &&
          r.Contact == null &&
          r.ProcessorId == "1" &&
          r.Expiration == DateTime.Today.AddDays(Constants.DEFAULT_DAYS))));
      }

      [TestMethod]
      public void ReturnsTheRegistrationDetails()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.LoadRegistration();

        Assert.AreEqual(registration, result);
      }

      [TestMethod]
      public void ChecksTheRegistrationDetailsIfValid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.LoadRegistration();

        checker.Verify(it => it.Check(registration));
      }
    }

    [TestClass]
    public class SaveRegistration : LicenserTests
    {
      [TestMethod]
      public void SavesTheRegistration()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(registration));
      }

      [TestMethod]
      public void SendsTheDetailsToTheServerIfInternallyValid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Submit(registration));
      }

      [TestMethod]
      public void DoesNotSendTheDetailsToTheServerIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Submit(It.IsAny<LicenseRegistration>()), Times.Never);
      }

      [TestMethod]
      public void AValidRemoteResponseAlsoSetsTheExpirationDateToTheNewValue()
      {
        var registration = ObjectMother.CreateRegistration();
        checker
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Expiration == new DateTime(9999, 12, 31))));
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void DoesNotSendLicenseToServerIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Submit(registration), Times.Never);
      }
    }

    [TestClass]
    public class Initialize : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        sut.Initialize();

        storage.Verify(it => it.Load());
      }

      [TestMethod]
      public void SavesANewRegistrationIfNoneExists()
      {
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r =>
          r.CreatedOn == DateTime.Today
          && r.Limits.Days == Constants.DEFAULT_DAYS
          && r.Limits.Runs == Constants.DEFAULT_RUNS - 1
          && r.Key == null
          && r.Name == null
          && r.Contact == null
          && r.ProcessorId == "1"
          && r.Expiration == DateTime.Today.AddDays(Constants.DEFAULT_DAYS))));
      }

      [TestMethod]
      public void UpdatesTheRemainingRunsIfGreaterThanZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 5;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Limits.Runs == 4)));
      }

      // IsLicensed

      [TestMethod]
      public void SetsIsLicensedToFalseIfThereAreNoRegistrationDetails()
      {
        sut.Initialize();

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfTheLicenseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.Initialize();

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void ChecksWithTheRemoteServerIfLicenseIsInternallyValid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        checker.Verify(it => it.Check(registration));
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfTheLicenseIsValid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        Assert.IsTrue(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfTheRemoteCheckFails()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        checker
          .Setup(it => it.Check(registration))
          .Returns((DateTime?) null);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfTheRemoteCheckReturnsAValidResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        checker
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        Assert.IsTrue(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfTheRemoteCheckReturnsAnExpirationDateInThePast()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        checker
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(2000, 1, 2));
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsTheExpirationDateToTheNewValueIfTheKeyIsCorrect()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        checker
          .Setup(it => it.Check(registration))
          .Returns(new DateTime(9999, 12, 31));
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Expiration == new DateTime(9999, 12, 31))));
      }

      // IsTrial

      [TestMethod]
      public void SetsIsTrialToTrueIfRegistrationIsValid()
      {
        validator
          .Setup(it => it.Isvalid(It.IsAny<LicenseRegistration>()))
          .Returns(true);

        sut.Initialize();

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfTheNumberOfDaysHasPassed()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits = new Limits {Days = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfTrialNotExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfRemainingRunsIsZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 0};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfRemainingRunsIsGreaterThanZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void IgnoresDaysIfMinusOne()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = -1, Runs = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void IgnoresRunsIfMinusOne()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = -1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void UpdatesRemainingRuns()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 2};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Limits.Runs == 1)));
      }

      [TestMethod]
      public void DoesNotUpdateRemainingRunsWhenZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 0};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        storage.Verify(it => it.Save(It.IsAny<LicenseRegistration>()), Times.Never);
      }

      [TestMethod]
      public void DoesNotUpdateRemainingRunsWhenMinusOne()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = -1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        storage.Verify(it => it.Save(It.IsAny<LicenseRegistration>()), Times.Never);
      }
    }

    //

    private class TestLicenser : Licenser
    {
      public TestLicenser(Storage storage, Sys sys, RemoteChecker checker, Validator validator)
        : base(storage, sys, checker, validator)
      {
      }

      // ReSharper disable once MemberHidesStaticFromOuterClass
      public new void Initialize()
      {
        base.Initialize();
      }
    }
  }
}