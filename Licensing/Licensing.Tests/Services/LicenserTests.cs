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
      public void SetsIsLicensedAndIsTrialToFalseIfNoneExists()
      {
        sut.LoadRegistration();

        Assert.IsFalse(sut.IsLicensed);
        Assert.IsFalse(sut.IsTrial);
      }

      [TestMethod]
      public void ReturnsNullIfNoRegistrationDetailsExist()
      {
        var result = sut.LoadRegistration();

        Assert.IsNull(result);
      }

      [TestMethod]
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

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
          .Returns(new LicenseStatus())
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
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

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
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

        sut.SaveRegistration(registration);

        storage.Verify(it => it.Save(registration));
      }

      [TestMethod]
      public void SubmitsTheDetailsToTheChecker()
      {
        var registration = ObjectMother.CreateRegistration();
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Submit(registration));
      }

      [TestMethod]
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

        sut.SaveRegistration(registration);

        checker.Verify(it => it.Check(registration));
      }
    }

    [TestClass]
    public class DeleteRegistration : LicenserTests
    {
      [TestMethod]
      public void DeletesTheRegistration()
      {
        sut.DeleteRegistration();

        storage.Verify(it => it.Delete());
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
      public void ChecksTheLicense()
      {
        var registration = ObjectMother.CreateRegistration();
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

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
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

        sut.Initialize();

        storage.Verify(it => it.Save(It.Is<LicenseRegistration>(r => r.Limits.Runs == 4)));
      }

      [TestMethod]
      public void DoesNotUpdateTheRemainingRunsIfNotGreaterThanZero()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 0;
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus());

        sut.Initialize();

        storage.Verify(it => it.Save(It.IsAny<LicenseRegistration>()), Times.Never);
      }

      [TestMethod]
      public void DoesNotUpdateTheRemainingRunsIfLicensed()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Limits.Runs = 5;
        storage
          .Setup(it => it.Load())
          .Returns(registration);
        checker
          .Setup(it => it.Check(registration))
          .Returns(new LicenseStatus {IsLicensed = true});

        sut.Initialize();

        storage.Verify(it => it.Save(It.IsAny<LicenseRegistration>()), Times.Never);
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