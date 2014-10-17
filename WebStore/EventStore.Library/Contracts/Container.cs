using System;

namespace EventStore.Library.Contracts
{
  public interface Container<TOut>
  {
    void Register<TIn>(Func<TOut> value);
    TOut Find<TIn>();
  }
}