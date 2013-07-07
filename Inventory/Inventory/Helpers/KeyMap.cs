using System;
using System.Collections.Generic;

namespace Renfield.Inventory.Helpers
{
  public class KeyMap<T>
  {
    public KeyMap()
    {
      map = new Dictionary<KeyStruct, T>();
    }

    public T GetValue(KeyStruct key, Func<T> f)
    {
      T value;
      if (!map.TryGetValue(key, out value))
      {
        value = f();
        map.Add(key, value);
      }

      return value;
    }

    //

    private readonly Dictionary<KeyStruct, T> map;
  }
}