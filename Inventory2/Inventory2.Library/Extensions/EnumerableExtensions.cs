using System;
using System.Collections.Generic;

namespace Inventory2.Library.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action(item);
                yield return item;
            }
        }
    }
}