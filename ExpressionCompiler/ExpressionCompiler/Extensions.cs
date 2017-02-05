using System;
using Functional.Maybe;

namespace ExpressionCompiler
{
    public static class Extensions
    {
        public static Maybe<int> TryParse(this string s)
        {
            return MaybeFunctionalWrappers
                .Wrap<string, int>(int.TryParse)
                .Invoke(s);
        }

        public static Maybe<T> Safe<T>(this Func<T> func)
        {
            return MaybeFunctionalWrappers
                .Catcher<object, T, Exception>(_ => func())
                .Invoke(null);
        }
    }
}