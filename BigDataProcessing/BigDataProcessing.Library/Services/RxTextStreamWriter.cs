using System;
using System.IO;
using System.Text;
using System.Threading;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxTextStreamWriter : RxTextWriter
  {
    public void Write(object destination, IObservable<string> data)
    {
      var stream = destination as Stream;
      if (stream == null)
        throw new ArgumentException("Invalid argument (expected stream)", nameof(destination));

      var handle = new ManualResetEvent(false);

      data.Subscribe(line => AppendLine(stream, line), () => handle.Set());

      handle.WaitOne();
      stream.Flush();
    }

    //

    private static void AppendLine(Stream stream, string line)
    {
      var buffer = Encoding.UTF8.GetBytes(line + Environment.NewLine);

      stream.Seek(0, SeekOrigin.End);
      stream.Write(buffer, 0, buffer.Length);
    }
  }
}