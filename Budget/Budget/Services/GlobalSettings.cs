using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Services
{
  public static class GlobalSettings
  {
    public static Func<DateTime> SystemTime = () => DateTime.Now;
  }
}