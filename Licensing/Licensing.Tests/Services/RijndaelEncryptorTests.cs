using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class RijndaelEncryptorTests
  {
    [TestInitialize]
    public void SetUp()
    {
      // generate all ASCII characters
      cleartext = new String(Enumerable.Range(0, 256).Select(i => (char) i).ToArray());
    }

    [TestMethod]
    public void EncryptingThenDecryptingReturnsOriginalResult()
    {
      var sut = new RijndaelEncryptor(PASSWORD, SALT);

      var encrypted = sut.Encrypt(cleartext);
      var decrypted = sut.Decrypt(encrypted);

      Assert.AreEqual(cleartext, decrypted);
    }

    [TestMethod]
    public void SameResultWithDifferentInstances()
    {
      var sut1 = new RijndaelEncryptor(PASSWORD, SALT);
      var encrypted = sut1.Encrypt(cleartext);

      var sut2 = new RijndaelEncryptor(PASSWORD, SALT);
      var decrypted = sut2.Decrypt(encrypted);

      Assert.AreEqual(cleartext, decrypted);
    }

    //

    private const string PASSWORD = "password";
    private const string SALT = "{E6661A5A-0304-4974-8AA1-B0F95A0212D4}";

    private string cleartext;
  }
}