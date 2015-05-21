using System;

namespace SocialNetwork.Library.Services
{
  public static class Sys
  {
    // ReSharper disable once InconsistentNaming
    public static Func<DateTime> Time = () => DateTime.Now;
  }
}