using System.Linq;
using System.Net;
using System.Threading;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.SafeRedir.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private const string BASE_URL = "http://localhost:8447";
    private const string URL = BASE_URL + "/";
    private const string REDIRECT_PREFIX = BASE_URL + "/r/";

    [TestMethod]
    public void IndexReturnsFormWithDefaultValues()
    {
      using (var web = new WebClient())
      {
        var root = LoadHtml("/");

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
        var submit = form.SelectSingleNode("//button[@type='submit']");
        Assert.IsNotNull(submit);
      }
    }

    [TestMethod]
    public void PostingToIndexReturnsNewUrlRedirectingToOriginal()
    {
      using (var web = new WebClient())
      {
        var data = string.Format("URL=example.com&SafeURL={0}&TTL={1}", "http://www.randomkittengenerator.com/", 300);
        web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        var shortened = web.UploadString(URL, data);
        Assert.IsTrue(shortened.StartsWith(REDIRECT_PREFIX));

        var res = SafeGet(shortened);
        Assert.AreEqual("Redirect", res.StatusCode.ToString());
        Assert.AreEqual("http://example.com/", res.Headers["Location"]);
      }
    }

    [TestMethod]
    public void PostingToIndexReturnsNewUrlRedirectingToSafeAfterTTL()
    {
      using (var web = new WebClient())
      {
        var data = string.Format("URL=example.com&SafeURL={0}&TTL={1}", "http://www.randomkittengenerator.com/", 1);
        web.Headers["Content-Type"] = "application/x-www-form-urlencoded";
        var shortened = web.UploadString(URL, data);
        Assert.IsTrue(shortened.StartsWith(REDIRECT_PREFIX));

        Thread.Sleep(1100); // sleep long enough for the TTL to expire

        var res = SafeGet(shortened);
        Assert.AreEqual("MovedPermanently", res.StatusCode.ToString());
        Assert.AreEqual("http://www.randomkittengenerator.com/", res.Headers["Location"]);
      }
    }

    [TestMethod]
    public void StatsReturnsNumberOfRecordsForSeveralPeriods()
    {
      var root = LoadHtml("/Home/Stats");

      var list = root.SelectSingleNode("//*[@id='stats']");
      var items = list.SelectNodes(".//li").ToList();

      Assert.AreEqual(4, items.Count);
      Assert.AreEqual("Today", items[0].SelectSingleNode(".//span[@class='period']").InnerText);
      Assert.IsNotNull(items[0].SelectSingleNode(".//span[@class='count']").InnerText);
      Assert.AreEqual("Current Month", items[1].SelectSingleNode(".//span[@class='period']").InnerText);
      Assert.IsNotNull(items[1].SelectSingleNode(".//span[@class='count']").InnerText);
      Assert.AreEqual("Current Year", items[2].SelectSingleNode(".//span[@class='period']").InnerText);
      Assert.IsNotNull(items[2].SelectSingleNode(".//span[@class='count']").InnerText);
      Assert.AreEqual("Overall", items[3].SelectSingleNode(".//span[@class='period']").InnerText);
      Assert.IsNotNull(items[3].SelectSingleNode(".//span[@class='count']").InnerText);
    }

    //

    private static HtmlNode LoadHtml(string page)
    {
      using (var web = new WebClient())
      {
        var html = web.DownloadString(BASE_URL + page);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return doc.DocumentNode;
      }
    }

    private static HttpWebResponse SafeGet(string url)
    {
      var req = (HttpWebRequest) WebRequest.Create(url);
      req.AllowAutoRedirect = false;

      return (HttpWebResponse) req.GetResponse();
    }
  }
}