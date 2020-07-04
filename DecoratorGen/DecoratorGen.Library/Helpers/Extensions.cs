using System;

namespace DecoratorGen.Library.Helpers
{
    public static class Extensions
    {
        public static TR Pipe<T, TR>(this T source, Func<T, TR> selector) =>
            selector(source);

        public static void Apply<T>(this T source, Action<T> action) =>
            action(source);
    }
}