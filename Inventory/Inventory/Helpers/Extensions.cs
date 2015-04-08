using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.Inventory.Helpers
{
  public static class Extensions
  {
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> multiSequence)
    {
      return multiSequence.SelectMany(s => s);
    }

    public static string Formatted(this decimal value)
    {
      return value.ToString(Constants.DECIMAL_FORMAT);
    }

    public static string Formatted(this decimal? value)
    {
      return value.HasValue ? value.Value.Formatted() : "";
    }

    public static DateTime? ParseDateNullable(this string s)
    {
      DateTime result;
      return DateTime.TryParse(s, out result) ? result : (DateTime?) null;
    }

    public static bool IsNullOrEmpty(this string s)
    {
      return string.IsNullOrEmpty(s);
    }

    public static string GetValue(this IDictionary<string, object> dict, string key)
    {
      object result;

      return dict.TryGetValue(key, out result) ? result + "" : null;
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> sequence)
    {
      return sequence ?? Enumerable.Empty<T>();
    }
  }
}