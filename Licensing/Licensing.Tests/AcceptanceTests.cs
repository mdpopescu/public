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
  }
}