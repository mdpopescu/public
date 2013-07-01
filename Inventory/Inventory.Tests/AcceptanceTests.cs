using System;
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

      var nav = root.GetTagById("ul", "menu");
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
      var root = LoadHtml("Stocks/");

      var productsTable = root.GetTagById("table", "products");
      Assert.IsNotNull(productsTable);
      CollectionAssert.AreEqual(new[] { "Name", "Quantity", "Recommended Retail Price", "Purchase Value", "Sale Value" }, productsTable.GetColumns());
    }

    [TestMethod]
    public void Acquisitions()
    {
      var root = LoadHtml("Acquisitions/");

      var linkToAdd = root.GetTagById("a", "acquisitions_create");
      Assert.IsNotNull(linkToAdd);
      Assert.AreEqual("/Acquisitions/Create", linkToAdd.Attributes["href"].Value);
      var mainTable = root.GetTagById("table", "acquisitions");
      Assert.IsNotNull(mainTable);
      CollectionAssert.AreEqual(new[] { "Company Name", "Date", "Total Value" }, mainTable.GetColumns());
      var itemsTable = root.GetTagById("table", "acquisition_items");
      Assert.IsNotNull(itemsTable);
      CollectionAssert.AreEqual(new[] { "Product Name", "Quantity", "Price", "Value" }, itemsTable.GetColumns());
    }

    [TestMethod]
    public void GetCreateAcquisitions()
    {
      var root = LoadHtml("Acquisitions/Create");

      var form = root.SelectSingleNode(".//form");
      var companyName = form.GetTagById("input", "companyName");
      Assert.IsNotNull(companyName);
      var date = form.GetTagById("input", "date");
      Assert.IsNotNull(date);
      Assert.AreEqual(DateTime.Today.ToString("MM/dd/yyyy"), date.Attributes["Value"].Value);
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