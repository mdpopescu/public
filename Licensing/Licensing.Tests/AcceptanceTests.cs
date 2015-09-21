using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private const string PASSWORD = "{B639CCED-AE77-4372-8BDC-F7CAFC396C1C}";
    private const string SALT = "{E705E430-83DD-4C76-A2F7-FB5B64D402C8}";
    private const string COMPANY = "ABC Ltd.";
    private const string PRODUCT = "Some product";

    private Licenser licenser;

    [TestInitialize]
    public void SetUp()
    {
      var options = new LicenseOptions
      {
        Password = PASSWORD,
        Salt = SALT,
        CheckUrl = null,
        SubmitUrl = null,
        Company = COMPANY,
        Product = PRODUCT,
      };

      licenser = Licenser.Create(options);
    }

    [TestCleanup]
    public void TearDown()
    {
      licenser.DeleteRegistration();
    }

    [TestMethod]
    public void ShouldRunIsFalseIfNoRegistrationIsFound()
    {
      Assert.IsFalse(licenser.ShouldRun);
    }

    [TestMethod]
    public void CreatesAndChecksLicense()
    {
      var registration = new LicenseRegistration
      {
        CreatedOn = new DateTime(2000, 1, 2),
        Limits = new Limits
        {
          Days = -1,
          Runs = -1,
        },
        Key = "{2686AC7F-2DD7-468C-908A-E28C8CC92A74}",
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        Expiration = new DateTime(9999, 12, 31),
      };

      licenser.SaveRegistration(registration);

      Assert.IsTrue(licenser.IsLicensed);
    }

    [TestMethod]
    public void CreatesAndChecksTrial()
    {
      var registration = new LicenseRegistration
      {
        CreatedOn = DateTime.Today,
        Limits = new Limits
        {
          Days = 10,
          Runs = -1,
        },
        Key = null,
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        Expiration = new DateTime(9999, 12, 31),
      };

      licenser.SaveRegistration(registration);

      Assert.IsFalse(licenser.IsLicensed);
      Assert.IsTrue(licenser.IsTrial);
    }

    [TestMethod]
    public void CreatesAndChecksExpiredTrial_1()
    {
      var registration = new LicenseRegistration
      {
        CreatedOn = new DateTime(2000, 1, 2),
        Limits = new Limits
        {
          Days = 10,
          Runs = -1,
        },
        Key = null,
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        Expiration = new DateTime(9999, 12, 31),
      };

      licenser.SaveRegistration(registration);

      Assert.IsFalse(licenser.IsLicensed);
      Assert.IsFalse(licenser.IsTrial);
    }

    [TestMethod]
    public void CreatesAndChecksExpiredTrial_2()
    {
      var registration = new LicenseRegistration
      {
        CreatedOn = DateTime.Today,
        Limits = new Limits
        {
          Days = 10,
          Runs = -1,
        },
        Key = null,
        Name = "Marcel",
        Contact = "mdpopescu@gmail.com",
        Expiration = new DateTime(2000, 1, 2),
      };

      licenser.SaveRegistration(registration);

      Assert.IsFalse(licenser.IsLicensed);
      Assert.IsFalse(licenser.IsTrial);
    }
  }
}