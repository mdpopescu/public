using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxGenericSplitter<TRecord> : RxSplitter<TRecord>
  {
    public IObservable<TRecord>[] Split(IObservable<TRecord> stream, int count, IScheduler scheduler)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      if (count <= 0)
        throw new ArgumentException("Value must be greater than zero.", nameof(count));

      var result = Enumerable
        .Range(0, count)
        .Select(_ => new Subject<TRecord>())
        .ToArray();

      //var current = 0;
      //stream
      //  .SubscribeOn(scheduler)
      //  .Subscribe(record =>
      //{
      //  result[current].OnNext(record);
      //  current = (current + 1) % count;
      //});

      //stream.Publish(obs => obs.Where((_, i) => ))

      //return result;

      return Enumerable
        .Range(0, count)
        .Select(i => stream.Where((_, index) => index % count == i))
        .ToArray();
    }
  }
}