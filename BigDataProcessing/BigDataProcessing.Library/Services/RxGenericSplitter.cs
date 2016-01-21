using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxGenericSplitter<TRecord> : RxSplitter<TRecord>
  {
    public IObservable<TRecord>[] Split(IEnumerator enumerator, int count)
    {
      if (enumerator == null)
        throw new ArgumentNullException(nameof(enumerator));
      if (count <= 0)
        throw new ArgumentException("Value must be greater than zero.", nameof(count));

      var gate = new object();
      Action<IObserver<TRecord>> processStream = o =>
      {
        while (true)
        {
          bool hasValues;
          var nextValue = default(TRecord);

          lock (gate)
          {
            hasValues = enumerator.MoveNext();
            if (hasValues)
              nextValue = (TRecord) enumerator.Current;
          }

          if (!hasValues)
            break;

          o.OnNext(nextValue);
        }
      };

      return Enumerable
        .Range(0, count)
        .Select(_ => Observable.Create<TRecord>(o => Task.Run(() => processStream(o))))
        .ToArray();

      //var streams = stream
      //  //.Publish()
      //  //.RefCount()
      //  .Select((record, index) => new { record, key = index % count })
      //  .GroupBy(it => it.key)
      //  .Select(g => g.Select(it => it.record));

      //return streams
      //  .ObserveOn(scheduler)
      //  .ToEnumerable()
      //  .ToArray();

      //var result = Enumerable
      //  .Range(0, count)
      //  .Select(_ => new Subject<TRecord>())
      //  .ToArray();

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

      //return Enumerable
      //  .Range(0, count)
      //  .Select(i => stream.Where((_, index) => index % count == i))
      //  .ToArray();
    }
  }
}