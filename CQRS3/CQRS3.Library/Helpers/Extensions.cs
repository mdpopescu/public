using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRS3.Library.Helpers
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var it in list)
                action(it);
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action)
        {
            return list.Select(
                it =>
                {
                    action(it);
                    return it;
                });
        }
    }
}