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
    [TestClass]
    public class IsLicensed : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        const string PASSWORD = "abc";

        var options = new LicenserOptions {Password = PASSWORD};
        var storage = new Mock<Storage>();
        var sut = new Licenser(options, storage.Object);

        sut.IsLicensed();

        storage.Verify(it => it.Load(PASSWORD));
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseKeyIsNull()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();
        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseKeyIsNotAGuid()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();

        var registration = ObjectMother.CreateRegistration();
        registration.Key = "abc";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsTrueIfTheLicenseKeyIsAGuid()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();

        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsTrue(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButNameIsEmptyOrNull()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();

        var registration = ObjectMother.CreateRegistration();
        registration.Name = "";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButContactIsEmptyOrNull()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();

        var registration = ObjectMother.CreateRegistration();
        registration.Contact = "";
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }

      [TestMethod]
      public void ReturnsFalseIfTheKeyIsvalidButHasExpired()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();

        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = new DateTime(2000, 1, 1);
        storage
          .Setup(it => it.Load(It.IsAny<string>()))
          .Returns(registration);

        var sut = new Licenser(options, storage.Object);

        var result = sut.IsLicensed();

        Assert.IsFalse(result);
      }
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