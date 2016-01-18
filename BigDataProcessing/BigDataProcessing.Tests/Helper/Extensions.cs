using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace BigDataProcessing.Tests.Helper
{
  public static class Extensions
  {
    public static List<T> ToList<T>(IObservable<T> obs)
    {
      return obs.ToEnumerable().ToList();
    }
  }
}