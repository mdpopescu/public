using System;

namespace WindowsFormsApp3.Extensions
{
    public static class Extensions
    {
        public static void IfNotNull<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
        }
    }
}