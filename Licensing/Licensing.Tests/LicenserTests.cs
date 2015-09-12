using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests
{
  [TestClass]
  public class LicenserTests
  {
    private LicenserOptions options;
    private Mock<Storage> storage;
    private Mock<Sys> sys;
    private Mock<Remote> remote;

    private Licenser sut;

    [TestInitialize]
    public void SetUp()
    {
      options = new LicenserOptions();
      storage = new Mock<Storage>();
      sys = new Mock<Sys>();
      remote = new Mock<Remote>();

      sut = new Licenser(options, storage.Object, sys.Object, remote.Object);
    }

    [TestClass]
    public class IsLicensed : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        const string PASSWORD = "abc";

        options.Password = PASSWORD;

        sut.IsLicensed();

        storage.Verify(it => it.Load(PASSWORD));
      }

      [TestMethod]
      public void ReturnsFalseIfThereAreNoRegistrationDetails()
      {
        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseKeyIsNull()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Key = null;
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseKeyIsNotAGuid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Key = "abc";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTheLicenseKeyIsAGuid()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButNameIsEmptyOrNull()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Name = "";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButContactIsEmptyOrNull()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Contact = "";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButHasExpired()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = new DateTime(2000, 1, 1);
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckFails()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Throws(new Exception());

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnEmptyString()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnInvalidResponse()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("xyz");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTheRemoteCheckReturnsAValidResponse()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        var result = sut.IsLicensed();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheRemoteCheckReturnsAnExpirationDateInThePast()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 2000-01-01");

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void RemoteCheckAlsoSetsTheExpirationDateToTheNewValue()
      {
        const string URL = "abc";

        options.CheckUrl = URL;
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");
        remote
          .Setup(it => it.Get(URL + "?Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"))
          .Returns("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

        sut.IsLicensed();

        storage.Verify(it => it.Save(It.IsAny<string>(), It.Is<LicenserRegistration>(r => r.Expiration == new DateTime(9999, 12, 31))));
      }
    }

    [TestClass]
    public class IsTrial : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        const string PASSWORD = "abc";

        options.Password = PASSWORD;

        sut.IsTrial();

        storage.Verify(it => it.Load(PASSWORD));
      }

      [TestMethod]
      public void ReturnsTrueIfLicensed()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
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
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var result = sut.IsTrial();

        Assert.IsTrue(result);
      }
    }

    [TestClass]
    public class ShouldRun : LicenserTests
    {
      //
    }

    [TestClass]
    public class ShowRegistration : LicenserTests
    {
      //
    }

    [TestClass]
    public class CreateRegistration : LicenserTests
    {
      //
    }
  }
}