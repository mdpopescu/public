using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using HtmlAgilityPack;
using Moq;

namespace Renfield.Inventory.Tests
{
  public static class Extensions
  {
    public static HtmlNode GetTagById(this HtmlNode node, string tag, string id)
    {
      return node.SelectSingleNode(string.Format(".//{0}[@id='{1}']", tag, id));
    }

    public static string[] GetColumns(this HtmlNode table)
    {
      return table
        .SelectNodes(".//th")
        .Select(node => node.InnerText)
        .ToArray();
    }

    public static void SetUpTable<TMock, TTable>(this Mock<TMock> mock, Expression<Func<TMock, IDbSet<TTable>>> expression, List<TTable> list)
      where TMock : class
      where TTable : class, new()
    {
      mock
        .Setup(expression)
        .Returns(new FakeDbSet<TTable>(list));
    }

    public static string Join(this IEnumerable<string> values, string separator)
    {
      return string.Join(separator, values);
    }
  }
}