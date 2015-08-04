using System;

namespace Acta.Library.Services
{
  public static class Global
  {
    public static Func<long> Time = () => DateTime.UtcNow.Ticks;
  }
}