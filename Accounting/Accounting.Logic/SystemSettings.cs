using System;

namespace Accounting.Logic
{
    public static class SystemSettings
    {
        // ReSharper disable once InconsistentNaming
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}