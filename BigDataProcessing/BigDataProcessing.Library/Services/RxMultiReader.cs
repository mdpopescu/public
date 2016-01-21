using System;
using BigDataProcessing.Library.Contracts;

namespace BigDataProcessing.Library.Services
{
  public class RxMultiReader<TSource, TRecord>
  {
    public RxMultiReader(Reader<TSource, TRecord> reader, RxSplitter<TRecord> splitter)
    {
      this.reader = reader;
      this.splitter = splitter;
    }

    public IObservable<TRecord>[] Read(TSource source, int count)
    {
      try
      {
        var records = reader.Read(source);
        return splitter.Split(records.GetEnumerator(), count);
      }
      catch
      {
        return null;
      }
    }

    //

    private readonly Reader<TSource, TRecord> reader;
    private readonly RxSplitter<TRecord> splitter;
  }
}