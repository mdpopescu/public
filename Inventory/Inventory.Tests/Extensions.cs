using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

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

    public static IEnumerable<T> FindByKey<T, TKey>(this IEnumerable<T> list, Func<T, TKey> keySelector, object key)
    {
      return list.Where(it => keySelector(it).Equals(key));
    }
  }
}