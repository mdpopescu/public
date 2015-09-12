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
    public class IsValid : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        const string PASSWORD = "abc";

        var options = new LicenserOptions {Password = PASSWORD};
        var storage = new Mock<Storage>();
        var sut = new Licenser(options, storage.Object);

        sut.IsValid();

        storage.Verify(it => it.Load(PASSWORD));
      }

      [TestMethod]
      public void ReturnsFalseIfTheLicenseKeyIsNull()
      {
        var options = new LicenserOptions();
        var storage = new Mock<Storage>();
        var sut = new Licenser(options, storage.Object);

        var result = sut.IsValid();

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