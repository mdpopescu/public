using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests
{
  [TestClass]
  public class SecureStorageTests
  {
    private SecureStorage sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new SecureStorage();
    }

    [TestClass]
    public class Load : SecureStorageTests
    {
      //
    }

    [TestClass]
    public class Save : SecureStorageTests
    {
      //
    }
  }
}