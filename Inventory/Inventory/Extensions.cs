using System.Collections.Generic;
using System.Linq;

namespace Renfield.Inventory
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

    public static string DataBinding(string listName, string field)
    {
      return string.Format("value: {1}, attr: {{ id : '{0}_' + $index() + '__{1}', name: '{0}[' + $index() + '].{1}' }}",
        listName, field);
    }
  }
}