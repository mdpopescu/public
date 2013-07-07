using System;

namespace Renfield.Inventory.Helpers
{
  public class Memoizer
  {
    public static Func<T> Memoize<T>(Func<T> f)
    {
      // do NOT inline - it will create a new one on each subsequent call and thus not cache anything
      var map = new KeyMap<T>();

      return () => map.GetValue(new KeyStruct(), f);
    }

    public static Func<T1, T> Memoize<T1, T>(Func<T1, T> f)
    {
      // do NOT inline - it will create a new one on each subsequent call and thus not cache anything
      var map = new KeyMap<T>();

      return arg => map.GetValue(new KeyStruct(arg), () => f(arg));
    }

    public static Func<T1, T2, T> Memoize<T1, T2, T>(Func<T1, T2, T> f)
    {
      // do NOT inline - it will create a new one on each subsequent call and thus not cache anything
      var map = new KeyMap<T>();

      return (a1, a2) => map.GetValue(new KeyStruct(a1, a2), () => f(a1, a2));
    }
  }
}