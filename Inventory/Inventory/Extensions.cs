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
  }
}