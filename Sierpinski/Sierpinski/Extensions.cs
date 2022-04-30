using System;
using System.Collections.Generic;

namespace Sierpinski
{
    public static class Extensions
    {
        public const int MAX_X = 1000;
        public const int MAX_Y = 1000;

        public static readonly Random RND = new();

        public static T PickRandom<T>(this IReadOnlyList<T> items) =>
            items[RND.Next(items.Count)];
    }
}