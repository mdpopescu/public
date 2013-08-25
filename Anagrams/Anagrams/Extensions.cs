using System.Collections.Generic;

namespace Renfield.Anagrams
{
  public static class Extensions
  {
    public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
      return dict.ContainsKey(key) ? dict[key] : default(TValue);
    }

    public static void Push<T>(this List<T> list, T value)
    {
      list.Add(value);
    }

    public static void Pop<T>(this List<T> list)
    {
      list.RemoveAt(list.Count - 1);
    }
  }
}