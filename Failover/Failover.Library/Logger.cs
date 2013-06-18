using System;

namespace Renfield.Failover.Library
{
  public interface Logger
  {
    void Debug(string message);
    void Error(Exception exception);
  }
}