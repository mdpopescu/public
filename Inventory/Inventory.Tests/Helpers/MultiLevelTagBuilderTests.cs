using System;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Inventory.Helpers;
using Renfield.Inventory.Tests.Properties;

namespace Renfield.Inventory.Tests.Helpers
{
  [TestClass]
  public class MultiLevelTagBuilderTests
  {
    [TestMethod]
    public void ToStringReturnsCorrectlyIndentedTable()
    {
      var table = new MultiLevelTagBuilder("table");
      table.MergeAttribute("id", "products");
      table.MergeAttribute("class", "pure-table pure-table-horizontal pure-table-striped");

      var thead = new MultiLevelTagBuilder("thead");
      var headTr = new MultiLevelTagBuilder("tr");
      headTr.Add(new TagBuilder("th") { InnerHtml = "header 1" });
      headTr.Add(new TagBuilder("th") { InnerHtml = "header 2" });
      headTr.Add(new TagBuilder("th") { InnerHtml = "header 3" });
      thead.Add(headTr);
      table.Add(thead);

      var tbody = new MultiLevelTagBuilder("tbody");
      tbody.MergeAttribute("data-bind", "foreach: data");
      var bodyTr = new MultiLevelTagBuilder("tr");
      bodyTr.Add(new TagBuilder("td") { InnerHtml = "field 1" });
      bodyTr.Add(new TagBuilder("td") { InnerHtml = "field 2" });
      bodyTr.Add(new TagBuilder("td") { InnerHtml = "field 3" });
      tbody.Add(bodyTr);
      table.Add(tbody);

      var result = table + Environment.NewLine;

      result.Should().Be(Resources.MLTB_table);
    }
  }
}