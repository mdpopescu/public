using System.Collections.Generic;
using System.Linq;

namespace CachingAlgorithms.Implementations
{
    public class MostRecentlyUsed<TKey, TValue> : BaseCache<TKey, TValue>
    {
        public MostRecentlyUsed(int capacity)
            : base(capacity)
        {
            seen = new List<TKey>(capacity);
        }

        public override void Put(TKey key, TValue value)
        {
            base.Put(key, value);

            seen.Insert(0, key);
        }

        //

        protected override void RemoveOldEntry()
        {
            if (seen.Count < capacity)
                return;

            var toRemove = seen.Last();
            seen.RemoveAt(seen.Count - 1);

            dict.Remove(toRemove);
        }

        //

        private readonly List<TKey> seen;
    }
}