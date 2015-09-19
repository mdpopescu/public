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
    private Mock<LicenseChecker> checker;

    private TestLicenser sut;

    [TestInitialize]
    public void SetUp()
    {
      storage = new Mock<Storage>();
      checker = new Mock<LicenseChecker>();

      sut = new TestLicenser(storage.Object, checker.Object);
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
        sut.LoadRegistration();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r =>
          r.CreatedOn == DateTime.Today &&
          r.Limits.Days == Constants.DEFAULT_DAYS &&
          r.Limits.Runs == Constants.DEFAULT_RUNS &&
          r.Key == null &&
          r.Name == null &&
          r.Contact == null &&
          r.Expiration == DateTime.Today.AddDays(Constants.DEFAULT_DAYS))));
      }

      [TestMethod]
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.LoadRegistration();

        checker.Verify(it => it.Check(registration));
      }

      [TestMethod]
      public void SavesTheRegistrationDetailsIfExpirationChanged()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = ObjectMother.OldDate;
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Callback<LicenseRegistration>(r => r.Expiration = ObjectMother.NewDate);

        sut.LoadRegistration();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Expiration == ObjectMother.NewDate)));
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
      public void SubmitsTheDetailsToTheChecker()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Submit(registration));
      }

      [TestMethod]
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Check(registration));
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
        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r =>
          r.CreatedOn == DateTime.Today
          && r.Limits.Days == Constants.DEFAULT_DAYS
          && r.Limits.Runs == Constants.DEFAULT_RUNS - 1
          && r.Key == null
          && r.Name == null
          && r.Contact == null
          && r.Expiration == DateTime.Today.AddDays(Constants.DEFAULT_DAYS))));
      }

      [TestMethod]
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);

        sut.Initialize();

        checker.Verify(it => it.Check(registration));
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
    }

    //

    private class TestLicenser : Licenser
    {
      public TestLicenser(Storage storage, LicenseChecker checker)
        : base(storage, checker)
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