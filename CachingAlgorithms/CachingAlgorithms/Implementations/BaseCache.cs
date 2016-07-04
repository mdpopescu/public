using System;
using System.Collections.Generic;
using CachingAlgorithms.Interfaces;

namespace CachingAlgorithms.Implementations
{
    public abstract class BaseCache<TKey, TValue> : Cache<TKey, TValue>
    {
        public virtual TValue Get(TKey key, Func<TKey, TValue> missing)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
                return value;

            value = missing(key);
            Put(key, value);

            return value;
        }

        public virtual void Put(TKey key, TValue value)
        {
            if (dict.Count >= capacity)
                RemoveOldEntry();

            dict.Add(key, value);
        }

        //

        protected int capacity;

        protected Dictionary<TKey, TValue> dict;

        protected BaseCache(int capacity)
        {
            this.capacity = capacity;

            dict = new Dictionary<TKey, TValue>(capacity);
        }

        protected abstract void RemoveOldEntry();
    }
}