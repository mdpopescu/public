using System;
using System.Collections.Generic;
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

        public static List<T> Left<T>(this List<T> list, int count) => count <= 0 ? new List<T>() : list.GetRange(0, count);
        public static List<T> Mid<T>(this List<T> list, int index) => index >= list.Count ? new List<T>() : list.GetRange(index, list.Count - index);
    }
}