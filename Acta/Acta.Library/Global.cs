using System;
using Acta.Library.Services;

namespace Acta.Library
{
  public static class Global
  {
    // ReSharper disable once InconsistentNaming
    public const string TYPE_KEY = "@TYPE";

    public static Func<long> Time = () => HighResolutionDateTime.UtcNow.Ticks;
  }
}