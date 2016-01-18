using System;
using System.IO;
using System.Reactive.Disposables;
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

      var reader = new StreamReader(stream);

      return Observable.Create<string>(o =>
      {
        string line;
        while ((line = reader.ReadLine()) != null)
          o.OnNext(line);

        o.OnCompleted();

        return Disposable.Empty;
      });
    }
  }
}