using System.Collections.Generic;
using System.Linq;

namespace CachingAlgorithms.Implementations
{
    public class LeastFutureUse<TKey, TValue> : BaseCache<TKey, TValue>
    {
        public LeastFutureUse(int capacity, TKey[] futureKeys)
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
                .GroupBy(key => key)
                .Select(g => new { key = g.Key, count = g.Count() })
                .OrderBy(it => it.count)
                .Select(it => it.key)
                .Take(1)
                .ToArray();

            if (toRemove.Any())
                dict.Remove(toRemove[0]);

            // remove the first future key, it had just been used
            futureKeys.RemoveFirst();
        }

        //

        private readonly LinkedList<TKey> futureKeys;
    }
}