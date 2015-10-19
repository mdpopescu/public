using Giles.Library.Models;
using Giles.Tests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Giles.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void CanCreatePostAndLabel()
    {
      var api = CreateApi();
      api.Login("test", "test");
      var entry = new Entry("subject", "message body", new[] { "label" });

      var id = api.CreateEntry("subject", "message body", new[] { "label" });

      var result = api.GetEntry(id);
      Assert.AreEqual(entry, result);
    }

    //

    private Helper.Api CreateApi()
    {
      return new ApiOverHttp("http://localhost:1234");
    }
  }
}