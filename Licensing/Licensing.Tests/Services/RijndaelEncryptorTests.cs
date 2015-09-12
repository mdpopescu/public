using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class RijndaelEncryptorTests
  {
    [TestMethod]
    public void EncryptingThenDecryptingReturnsOriginalResult()
    {
      var sut = new RijndaelEncryptor("password");

      // generate all ASCII characters
      var original = new String(Enumerable.Range(0, 256).Select(i => (char) i).ToArray());

      var encrypted = sut.Encrypt(original);
      var decrypted = sut.Decrypt(encrypted);

      Assert.AreEqual(original, decrypted);
    }
  }
}