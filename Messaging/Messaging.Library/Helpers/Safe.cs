using System;

namespace Messaging.Library.Helpers
{
    public static class Safe
    {
        public static T? Call<T>(Func<T> func, T? defValue = default)
        {
            try
            {
                return func();
            }
            catch
            {
                return defValue;
            }
        }
    }
}