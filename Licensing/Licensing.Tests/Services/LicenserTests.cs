using System;
using System.Collections.Generic;
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
    private Mock<Validator> validator;
    private Mock<Remote> remote;

    private TestLicenser sut;

    [TestInitialize]
    public void SetUp()
    {
      storage = new Mock<Storage>();
      sys = new Mock<Sys>();
      remote = new Mock<Remote>();
      validator = new Mock<Validator>();

      sut = new TestLicenser(storage.Object, sys.Object, validator.Object) {Remote = remote.Object, ResponseParser = new ResponseParserImpl()};
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
        sut.Remote = null;

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
      public void SetsIsLicensedToTrueIfTheLicenseIsValidAndNoRemoteCheck()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sut.Remote = null;

        sut.Initialize();

        Assert.IsTrue(sut.IsLicensed);
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

        sut.Initialize();

        remote.Verify(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"));
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
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns((string) null);

        sut.Initialize();

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfTheRemoteCheckReturnsAnInvalidResponse()
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
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

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
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 2000-01-01");

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
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

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

    [TestClass]
    public class SaveRegistration : LicenserTests
    {
      [TestMethod]
      public void GetsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.SaveRegistration(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void SetsTheProcessorIdInTheRegistrationDetails()
      {
        var registration = ObjectMother.CreateRegistration();
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
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
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
        sut.Remote = null;

        sut.SaveRegistration(registration);

        remote.Verify(it => it.Post(It.IsAny<string>()), Times.Never);
      }

      [TestMethod]
      public void DoesNotTryToEncodeTheFieldsIfNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        sut.Remote = null;

        sut.SaveRegistration(registration);

        sys.Verify(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never);
      }

      [TestMethod]
      public void SavesTheRegistrationIfTheServerReturnedAValidResponse()
      {
        var registration = ObjectMother.CreateRegistration();
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
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

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
      public void SetsIsTrialToTrueIfLicenseInvalidButTrialOk()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SetsIsTrialToFalseIfBothLicenseAndTrialInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Days = 0;
        registration.Limits.Runs = 0;
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void DoesNotSendLicenseToServerIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        remote.Verify(it => it.Post(It.IsAny<string>()), Times.Never);
      }

      [TestMethod]
      public void SavesLicenseToStorageIfInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(false);

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(registration));
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfValidAndNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sut.Remote = null;

        sut.SaveRegistration(registration);

        Assert.IsTrue(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsTrialToTrueIfValidAndNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sut.Remote = null;

        sut.SaveRegistration(registration);

        Assert.IsTrue(sut.IsTrial);
      }

      [TestMethod]
      public void SavesLicenseToStorageIfValidAndNoRemote()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sut.Remote = null;

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(registration));
      }

      [TestMethod]
      public void SetsIsLicensedToTrueIfRemoteReturnsOk()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.SaveRegistration(registration);

        Assert.IsTrue(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfRemoteReturnsInvalidFromPost()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("");
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.SaveRegistration(registration);

        Assert.IsFalse(sut.IsLicensed);
      }

      [TestMethod]
      public void SetsIsLicensedToFalseIfRemoteReturnsInvalidFromGet()
      {
        var registration = ObjectMother.CreateRegistration();
        validator
          .Setup(it => it.Isvalid(registration))
          .Returns(true);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");
        remote
          .Setup(it => it.Post("abc"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("");

        sut.SaveRegistration(registration);

        Assert.IsFalse(sut.IsLicensed);
      }
    }

    //

    private class TestLicenser : Licenser
    {
      public TestLicenser(Storage storage, Sys sys, Validator validator)
        : base(storage, sys, validator)
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