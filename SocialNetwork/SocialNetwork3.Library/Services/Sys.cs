using System;

namespace SocialNetwork3.Library.Services
{
    public static class Sys
    {
        /// <summary>
        /// Returns the current time; settable property to enable testing.
        /// </summary>
        public static Func<DateTime> Time { get; set; } = () => DateTime.Now;
    }
}