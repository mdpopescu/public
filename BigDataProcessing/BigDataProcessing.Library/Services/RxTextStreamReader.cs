using System;
using System.IO;
using System.Reactive.Linq;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxTextStreamReader : RxTextReader
  {
    public IObservable<string> Read(object source)
    {
      var stream = source as Stream;
      if (stream == null)
        throw new ArgumentException("Invalid argument (expected stream)", nameof(source));

      return Observable.Create<string>(o =>
      {
        var reader = new StreamReader(stream);

        string line;
        while ((line = reader.ReadLine()) != null)
          o.OnNext(line);

        o.OnCompleted();

        // Disposing the reader also disposes the underlying stream
        return reader;
      });
    }
  }
}