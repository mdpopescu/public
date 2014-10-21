using System;

namespace FindDuplicates.Contracts
{
  public interface Cache<TKey, TValue>
  {
    TValue Get(TKey key, Func<TKey, TValue> func);
  }
}