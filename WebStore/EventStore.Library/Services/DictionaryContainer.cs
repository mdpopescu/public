using System;
using System.Collections.Generic;
using EventStore.Library.Contracts;

namespace EventStore.Library.Services
{
  public class DictionaryContainer<TOut> : Container<TOut>
  {
    public DictionaryContainer(TOut def)
    {
      this.def = def;
    }

    public void Register<TIn>(Func<TOut> value)
    {
      dict[typeof (TIn)] = value;
    }

    public TOut Find<TIn>()
    {
      var key = typeof (TIn);
      return dict.ContainsKey(key) ? dict[key]() : def;
    }

    //

    private readonly TOut def;
    private readonly Dictionary<Type, Func<TOut>> dict = new Dictionary<Type, Func<TOut>>();
  }
}