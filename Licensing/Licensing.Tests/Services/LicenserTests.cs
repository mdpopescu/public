using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    private Mock<Validator> validator;
    private Mock<Remote> remote;

    private Licenser sut;

    [TestInitialize]
    public void SetUp()
    {
      storage = new Mock<Storage>();
      sys = new Mock<Sys>();
      remote = new Mock<Remote>();
      validator = new Mock<Validator>();

      sut = new Licenser(storage.Object, sys.Object, validator.Object) {Remote = remote.Object, ResponseParser = new ResponseParserImpl()};
    }

    [TestClass]
    public class IsLicensed : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        sut.IsLicensed();

        storage.Verify(it => it.Load());
      }

      [TestMethod]
      public void ReturnsFalseIfThereAreNoRegistrationDetails()
      {
        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTheLicenseIsValidAndNoRemoteCheck()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sut.Remote = null;

        var result = sut.IsLicensed();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ChecksWithTheRemoteServer()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);

        sut.IsLicensed();

        remote.Verify(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"));
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckFails()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Throws(new Exception());

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnEmptyString()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnInvalidResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("xyz");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTheRemoteCheckReturnsAValidResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        var result = sut.IsLicensed();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnExpirationDateInThePast()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 2000-01-01");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void AValidRemoteResponseAlsoSetsTheExpirationDateToTheNewValue()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.IsLicensed();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Expiration == new DateTime(9999, 12, 31))));
      }
    }

    [TestClass]
    public class IsTrial : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        sut.IsTrial();

        storage.Verify(it => it.Load());
      }

      [TestMethod]
      public void ReturnsTrueIfLicensed()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfThereAreNoRegistrationDetails()
      {
        var result = sut.IsTrial();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheLimitsObjectIsNotSet()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits = null;
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheNumberOfDaysHasPassed()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = new DateTime(2000, 1, 1);
        registration.Limits = new Limits {Days = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTrialNotExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfRemainingRunsIsZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 0};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfRemainingRunsIsGreaterThanZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.CreatedOn = DateTime.Today;
        registration.Limits = new Limits {Days = 1, Runs = 1};
        registration.Key = null;
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsTrue(result);
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

        var result = sut.IsTrial();

        Assert.IsTrue(result);
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

        var result = sut.IsTrial();

        Assert.IsTrue(result);
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

        sut.IsTrial();

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

        sut.IsTrial();

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

        sut.IsTrial();

        storage.Verify(it => it.Save(It.IsAny<LicenseRegistration>()), Times.Never);
      }
    }

    [TestClass]
    public class SaveRegistration : LicenserTests
    {
      [TestMethod]
      public void GetsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.SaveRegistration(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void SetsTheProcessorIdInTheRegistrationDetails()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("2");

        sut.SaveRegistration(registration);

        Assert.AreEqual("2", registration.ProcessorId);
      }

      [TestMethod]
      public void SendsTheDetailsToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");

        sut.SaveRegistration(registration);

        remote.Verify(it => it.Post("abc"));
      }

      [TestMethod]
      public void DoesNotSendTheDetailsToTheServerIfNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sut.Remote = null;

        sut.SaveRegistration(registration);

        remote.Verify(it => it.Post(It.IsAny<string>()), Times.Never);
      }

      [TestMethod]
      public void DoesNotTryToEncodeTheFieldsIfNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sut.Remote = null;

        sut.SaveRegistration(registration);

        sys.Verify(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never);
      }

      [TestMethod]
      public void SavesTheRegistrationIfTheServerReturnedAValidResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(registration));
      }

      [TestMethod]
      public void AValidRemoteResponseAlsoSetsTheExpirationDateToTheNewValue()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Expiration == new DateTime(9999, 12, 31))));
      }
    }
  }
}