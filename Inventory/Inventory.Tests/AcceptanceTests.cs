using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using FluentAssertions;
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
      nav.Should().NotBeNull();

      var links = nav.SelectNodes(".//li/a").ToList();
      links.Should().HaveCount(4);

      var linksText = links.Select(it => it.InnerText).ToList();
      linksText.ShouldAllBeEquivalentTo(new[] { "Home", "Product Inventory", "Acquisitions", "Sales" });
      //Assert.AreEqual("Products", links[4].InnerText);
      //Assert.AreEqual("Companies", links[5].InnerText);
      //Assert.AreEqual("Todos", links[6].InnerText);
    }

    [TestMethod]
    public void Stock()
    {
      var root = LoadHtml("Stocks/");

      var productsTable = root.GetTagById("table", "products");
      productsTable.Should().NotBeNull();

      var columns = productsTable.GetColumns();
      columns.ShouldAllBeEquivalentTo(new[] { "Name", "Quantity", "Recommended Retail Price", "Purchase Value", "Sale Value" });
    }

    [TestMethod]
    public void Acquisitions()
    {
      var root = LoadHtml("Acquisitions/");

      var linkToAdd = root.GetTagById("a", "acquisitions_create");
      linkToAdd.Should().NotBeNull();
      linkToAdd.Attributes["href"].Value.Should().Be("/Acquisitions/Create");

      var mainTable = root.GetTagById("table", "acquisitions");
      mainTable.Should().NotBeNull();
      mainTable.GetColumns().ShouldAllBeEquivalentTo(new[] { "Company Name", "Date", "Total Value" });

      var itemsTable = root.GetTagById("table", "acquisition_items");
      itemsTable.Should().NotBeNull();
      itemsTable.GetColumns().ShouldAllBeEquivalentTo(new[] { "Product Name", "Quantity", "Price", "Value" });
    }

    [TestMethod]
    public void GetCreateAcquisitions()
    {
      var root = LoadHtml("Acquisitions/Create");

      var companyName = root.GetTagById("input", "CompanyName");
      companyName.Should().NotBeNull();

      var date = root.GetTagById("input", "Date");
      date.Should().NotBeNull();
      date.Attributes["Value"].Value.Should().Be(DateTime.Today.ToString("MM/dd/yyyy"));
    }

    [TestMethod]
    [Ignore]
    public void PostCreateAcquisitionsRedirectsBackToGet()
    {
      var postData = new NameValueCollection
      {
        { "CompanyName", "Microsoft" },
        { "Date", "07/01/2013" },
        { "Items[0].ProductName", "Hammer" },
        { "Items[0].Quantity", "1" },
        { "Items[0].Price", "4" },
      };

      var res = SafePost("Acquisitions/Create", postData);
      res.StatusCode.Should().Be("Redirect");
      res.Headers["Location"].Should().Be("/Acquisitions/Create");
    }

    [TestMethod]
    public void Sales()
    {
      var root = LoadHtml("Sales/");

      var linkToAdd = root.GetTagById("a", "sales_create");
      linkToAdd.Should().NotBeNull();
      linkToAdd.Attributes["href"].Value.Should().Be("/Sales/Create");

      var mainTable = root.GetTagById("table", "sales");
      mainTable.Should().NotBeNull();
      mainTable.GetColumns().ShouldAllBeEquivalentTo(new[] { "Company Name", "Date", "Total Value" });

      var itemsTable = root.GetTagById("table", "sale_items");
      itemsTable.Should().NotBeNull();
      itemsTable.GetColumns().ShouldAllBeEquivalentTo(new[] { "Product Name", "Quantity", "Price", "Value" });
    }

    [TestMethod]
    public void GetCreateSales()
    {
      var root = LoadHtml("Sales/Create");

      var companyName = root.GetTagById("input", "CompanyName");
      companyName.Should().NotBeNull();

      var date = root.GetTagById("input", "Date");
      date.Should().NotBeNull();
      date.Attributes["Value"].Value.Should().Be(DateTime.Today.ToString("MM/dd/yyyy"));
    }

    [TestMethod]
    [Ignore]
    public void PostCreateSalesRedirectsBackToGet()
    {
      var postData = new NameValueCollection
      {
        { "CompanyName", "Microsoft" },
        { "Date", "07/01/2013" },
        { "Items[0].ProductName", "Hammer" },
        { "Items[0].Quantity", "1" },
        { "Items[0].Price", "4" },
      };

      var res = SafePost("Sales/Create", postData);
      res.StatusCode.Should().Be("Redirect");
      res.Headers["Location"].Should().Be("/Sales/Create");
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

    private static HttpWebResponse SafePost(string page, NameValueCollection values)
    {
      var byteArray = BuildPOSTData(values);

      var req = (HttpWebRequest) WebRequest.Create(string.Format("{0}/{1}", BASE_URL, page));
      req.AllowAutoRedirect = false;
      req.Method = "POST";
      req.ContentType = "application/x-www-form-urlencoded";
      req.ContentLength = byteArray.Length;

      var stream = req.GetRequestStream();
      stream.Write(byteArray, 0, byteArray.Length);

      return (HttpWebResponse) req.GetResponse();
    }

    private static byte[] BuildPOSTData(NameValueCollection values)
    {
      var data = values
        .Cast<string>()
        .Select(key => new KeyValuePair<string, string>(key, values[key]))
        .Select(v => string.Format("{0}={1}", v.Key, HttpUtility.UrlEncode(v.Value)))
        .Join("&");

      return Encoding.UTF8.GetBytes(data + "");
    }
  }
}