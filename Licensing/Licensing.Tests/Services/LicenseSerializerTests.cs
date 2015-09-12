using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class LicenseSerializerTests
  {
    [TestMethod]
    public void SerializingThenDeserializingReturnsTheSameResult()
    {
      var sut = new LicenseSerializer();

      var details = ObjectMother.CreateRegistration();
      var serialized = sut.Serialize(details);
      var result = sut.Deserialize(serialized);

      Assert.AreEqual(new DateTime(2000, 1, 1), result.CreatedOn);
      Assert.AreEqual(-1, result.Limits.Days);
      Assert.AreEqual(-1, result.Limits.Runs);
      Assert.AreEqual("{D98F6376-94F7-4D82-AA37-FC00F0166700}", result.Key);
      Assert.AreEqual("Marcel", result.Name);
      Assert.AreEqual("mdpopescu@gmail.com", result.Contact);
      Assert.AreEqual(new DateTime(9999, 12, 31), result.Expiration);
    }
  }
}