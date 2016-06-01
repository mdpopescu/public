using System;

namespace SocialNetwork2.Library.Implementations
{
    public static class Sys
    {
        // ReSharper disable once InconsistentNaming
        public static Func<DateTime> Time = () => DateTime.Now;
    }
}