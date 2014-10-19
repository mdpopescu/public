using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Tests.Models.Events;

namespace WebStore.Tests.Services
{
  [TestClass]
  public class StringEventSerializerTests
  {
    [TestMethod]
    public void SerializesAndDeserializesAnEvent()
    {
      var ev = new ProductSoldEvent { Name = "test", Quantity = 12.34m };
      var sut = new StringEventSerializer();

      var result = sut.Deserialize(sut.Serialize(ev)) as ProductSoldEvent;

      Assert.IsNotNull(result);
      Assert.AreEqual("test", result.Name);
      Assert.AreEqual(12.34m, result.Quantity);
    }
  }
}