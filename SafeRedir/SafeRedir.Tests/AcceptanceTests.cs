using System.Net;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.SafeRedir.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private const string BASE_URL = "http://localhost:8447";

    [TestClass]
    public class Index : AcceptanceTests
    {
      private const string URL = BASE_URL + "/";

      [TestMethod]
      public void ReturnsFormWithDefaultValues()
      {
        using (var web = new WebClient())
        {
          var html = web.DownloadString(URL);
          var doc = new HtmlDocument();
          doc.LoadHtml(html);

          var root = doc.DocumentNode;
          var form = root.SelectSingleNode("//form");
          Assert.IsNotNull(form);
          var url = form.SelectSingleNode("//input[@id='URL']");
          Assert.IsNotNull(url);
          Assert.AreEqual("", url.Attributes["Value"].Value);
          var safeUrl = form.SelectSingleNode("//input[@id='SafeURL']");
          Assert.IsNotNull(safeUrl);
          Assert.AreEqual("http://www.randomkittengenerator.com/", safeUrl.Attributes["Value"].Value);
          var expiresInMins = form.SelectSingleNode("//input[@id='TTL']");
          Assert.IsNotNull(expiresInMins);
          Assert.AreEqual("300", expiresInMins.Attributes["Value"].Value);
          var submit = form.SelectSingleNode("//input[@type='submit']");
          Assert.IsNotNull(submit);
        }
      }
    }
  }
}