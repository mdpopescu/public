using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsClock.Contracts;

namespace WinFormsClock.Implementations
{
    public class Cache<TItem, TKey> : ICache<TItem, TKey>
        where TItem : IDisposable
    {
        public Cache(int limit)
        {
            this.limit = limit;

            items = new Dictionary<TKey, TItem>(limit);
        }

        public void Dispose()
        {
            foreach (var item in items.Values)
                item.Dispose();
        }

        public TItem Get(TKey key, Func<TItem> constructor)
        {
            if (items.ContainsKey(key))
                return items[key];

            var newItem = constructor();
            AddItem(key, newItem);
            return newItem;
        }

        //

        private readonly int limit;

        private readonly IDictionary<TKey, TItem> items;

        private void AddItem(TKey key, TItem item)
        {
            if (items.Count >= limit)
                items.Remove(items.Keys.First());

            items.Add(key, item);
        }
    }
}