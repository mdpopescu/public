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

      Assert.AreEqual(ObjectMother.OldDate, result.CreatedOn);
      Assert.AreEqual(-1, result.Limits.Days);
      Assert.AreEqual(-1, result.Limits.Runs);
      Assert.AreEqual(ObjectMother.KEY, result.Key);
      Assert.AreEqual(ObjectMother.NAME, result.Name);
      Assert.AreEqual(ObjectMother.CONTACT, result.Contact);
      Assert.AreEqual(ObjectMother.FutureDate, result.Expiration);
    }
  }
}