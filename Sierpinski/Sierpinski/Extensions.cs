using System;
using System.Collections.Generic;

namespace Sierpinski
{
    public static class Extensions
    {
        public static readonly Random RND = new();

        public static T PickRandom<T>(this IReadOnlyList<T> items) =>
            items[RND.Next(items.Count)];
    }
}