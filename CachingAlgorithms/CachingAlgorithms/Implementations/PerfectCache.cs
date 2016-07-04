using System.Collections.Generic;
using System.Linq;

namespace CachingAlgorithms.Implementations
{
    public class PerfectCache<TKey, TValue> : BaseCache<TKey, TValue>
    {
        public PerfectCache(int capacity, TKey[] futureKeys)
            : base(capacity)
        {
            this.futureKeys = new LinkedList<TKey>(futureKeys);
        }

        //

        protected override void RemoveOldEntry()
        {
            if (futureKeys.First == null)
                return;

            var toRemove = futureKeys
                .Select((key, index) => new { key, index })
                .GroupBy(it => it.key)
                .Select(g => new { key = g.Key, index = g.Select(it => it.index).Min() })
                .OrderByDescending(it => it.index)
                .Take(1)
                .ToArray();

            if (toRemove.Any())
                dict.Remove(toRemove[0].key);

            // remove the first future key, it had just been used
            futureKeys.RemoveFirst();
        }

        //

        private readonly LinkedList<TKey> futureKeys;
    }
}