using System.Collections;
using System.Collections.Generic;
using Messaging.Library.Contracts;

namespace Messaging.Library.Services
{
    public class BoundedCollection<T> : ICollection<T>
    {
        public int Count => collection.Count;
        public bool IsReadOnly => collection.IsReadOnly;

        public BoundedCollection(ICollection<T> collection, int maxCount, IBackpressurePolicy<T> policy)
        {
            this.collection = collection;
            this.maxCount = maxCount;
            this.policy = policy;
        }

        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            // overflow if already at or over maxCount
            var newItem = collection.Count >= maxCount ? policy.HandleOverflow(item) : item;

            // do not add if null
            if (newItem is not null)
                collection.Add(newItem);
        }

        public void Clear()
        {
            collection.Clear();
            policy.Release(); // no overflow if the collection is cleared
        }

        public bool Contains(T item) => collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var result = collection.Remove(item);

            // no overflow if the count is less than 90% of the max
            if (collection.Count < 0.9 * maxCount)
                policy.Release();

            return result;
        }

        //

        private readonly ICollection<T> collection;
        private readonly int maxCount;
        private readonly IBackpressurePolicy<T> policy;
    }
}