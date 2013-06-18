using System;
using System.IO;

namespace Renfield.Failover.Library
{
  public class StreamLogger : Logger
  {
    public Func<DateTime> SystemClock = () => DateTime.Now;

    public StreamLogger(Stream stream)
    {
      writer = new StreamWriter(stream) { AutoFlush = true };
    }

    public void Debug(string message)
    {
      WriteMessage("D", message);
    }

    public void Error(Exception exception)
    {
      WriteMessage("E", exception.Message);
    }

    //

    private readonly StreamWriter writer;

    private void WriteMessage(string messageType, string message)
    {
      writer.WriteLine("[{0:yyyy.MM.dd HH:mm:ss} {1}] {2}", SystemClock.Invoke(), messageType, message);
    }
  }
}