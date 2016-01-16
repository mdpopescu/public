using System;

namespace BigDataProcessing.Library.Contracts
{
  public interface RxReader<in TSource, out TRecord>
  {
    IObservable<TRecord> Read(TSource source);
  }
}