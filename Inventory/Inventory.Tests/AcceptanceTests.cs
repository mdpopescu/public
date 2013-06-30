using System.Linq;
using System.Net;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Renfield.Inventory.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    private const string BASE_URL = "http://localhost:62011/";

    [TestMethod]
    public void HomePageElements()
    {
      var root = LoadHtml("");

      var nav = root.SelectSingleNode(".//ul[@id='menu']");
      Assert.IsNotNull(nav);
      var links = nav.SelectNodes(".//li/a").ToList();
      Assert.AreEqual(7, links.Count);
      Assert.AreEqual("Home", links[0].InnerText);
      Assert.AreEqual("Product Inventory", links[1].InnerText);
      Assert.AreEqual("Acquisitions", links[2].InnerText);
      Assert.AreEqual("Sales", links[3].InnerText);
      Assert.AreEqual("Products", links[4].InnerText);
      Assert.AreEqual("Companies", links[5].InnerText);
      Assert.AreEqual("Todos", links[6].InnerText);
    }

    [TestMethod]
    public void Stock()
    {
      var root = LoadHtml("Stock/");

      var productsTable = root.SelectSingleNode(".//table[@id='products']");
      Assert.IsNotNull(productsTable);
      var columns = productsTable
        .SelectNodes(".//th")
        .Select(node => node.InnerText)
        .ToArray();
      CollectionAssert.AreEqual(new[] { "Name", "Quantity", "Purchase Value", "Sale Value" }, columns);
    }

    //

    private static HtmlNode LoadHtml(string page)
    {
      var html = Get(BASE_URL + page);
      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      return doc.DocumentNode;
    }

    private static string Get(string url)
    {
      using (var web = new WebClient())
        return web.DownloadString(url);
    }
  }
}