using System;

namespace BigDataProcessing.Library.Contracts
{
  public interface RxWriter<in TDestination, in TRecord>
  {
    void Write(TDestination destination, IObservable<TRecord> data);
  }
}