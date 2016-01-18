using System;
using System.Reactive.Concurrency;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxMultiReader<TSource, TRecord>
  {
    public RxMultiReader(RxReader<TSource, TRecord> reader, RxSplitter<TRecord> splitter)
    {
      this.reader = reader;
      this.splitter = splitter;
    }

    public IObservable<TRecord>[] Read(TSource source, int count, IScheduler scheduler)
    {
      try
      {
        return splitter.Split(reader.Read(source), count, scheduler);
      }
      catch
      {
        return null;
      }
    }

    //

    private readonly RxReader<TSource, TRecord> reader;
    private readonly RxSplitter<TRecord> splitter;
  }
}