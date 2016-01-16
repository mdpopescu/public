using System;
using System.IO;
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

      var writer = new StreamWriter(stream);
      data.Subscribe(writer.WriteLine, () => writer.Flush());
    }
  }
}