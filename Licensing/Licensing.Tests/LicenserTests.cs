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
    public class Check : LicenserTests
    {
      [TestMethod]
      public void LoadsRegistrationDetailsFromStorage()
      {
        const string PASSWORD = "abc";

        var options = new LicenserOptions {Password = PASSWORD};
        var storage = new Mock<Storage>();
        var sut = new Licenser(options, storage.Object);

        sut.Check();

        storage.Verify(it => it.Load(PASSWORD));
      }
    }

    [TestClass]
    public class ShowRegistration : LicenserTests
    {
      //
    }
  }
}