using System;
using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library.Helpers
{
  public static class Generators
  {
    public static IEnumerable<T> While<T>(Func<T> getNext, Predicate<T> predicate)
    {
      var next = getNext();
      while (predicate(next))
      {
        yield return next;
        next = getNext();
      }
    }

    public static IEnumerable<T> Until<T>(Func<T> getNext, Predicate<T> predicate)
    {
      T next;
      do
      {
        next = getNext();
        yield return next;
      } while (!predicate(next));
    }
  }
}