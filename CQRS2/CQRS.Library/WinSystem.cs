using System;

namespace CQRS.Library
{
  public static class WinSystem
  {
    // ReSharper disable once InconsistentNaming FieldCanBeMadeReadOnly.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public static Action Terminate = () => Environment.Exit(-1);
  }
}