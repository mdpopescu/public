using System;

namespace CachingAlgorithms.Interfaces
{
    public interface Cache<TKey, TValue>
    {
        TValue Get(TKey key, Func<TKey, TValue> missing);
        void Put(TKey key, TValue value);
    }
}