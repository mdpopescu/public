using System;
using System.Runtime.InteropServices;

namespace Acta.Library.Services
{
    // based on http://manski.net/2014/07/high-resolution-clock-in-csharp/
    public static class HighResolutionDateTime
    {
        static HighResolutionDateTime()
        {
            try
            {
                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                IsAvailable = true;
            }
            catch (EntryPointNotFoundException)
            {
                // Not running Windows 8 or higher.
                IsAvailable = false;
            }
        }

        public static DateTime UtcNow
        {
            get
            {
                if (!IsAvailable)
                    return DateTime.UtcNow;

                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);
                return DateTime.FromFileTimeUtc(filetime);
            }
        }

        //

        private static bool IsAvailable { get; }

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);
    }
}