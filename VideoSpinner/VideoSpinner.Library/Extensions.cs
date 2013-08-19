using System.Collections.Generic;

namespace Renfield.VideoSpinner.Library
{
    public static class Extensions
    {
        public static IEnumerable<T> RunningSum<T>(this IEnumerable<T> sequence)
        {
            dynamic sum = default(T);
            foreach (var item in sequence)
            {
                sum = sum + item;
                yield return sum;
            }
        }
    }
}