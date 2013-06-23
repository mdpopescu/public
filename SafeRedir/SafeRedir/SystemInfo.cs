using System;

namespace Renfield.SafeRedir
{
  public static class SystemInfo
  {
    public static Func<DateTime> SystemClock = () => DateTime.Now;
  }
}