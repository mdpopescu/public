using System;
using System.Reactive.Concurrency;

namespace BigDataProcessing.Library.Contracts
{
  public interface RxSplitter<TRecord>
  {
    /// <summary>
    ///   Splits the input stream into <see cref="count" /> concurrent output streams.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="count">The number of output streams.</param>
    /// <param name="scheduler">The scheduler used to split the input stream (so that it doesn't freeze the calling thread).</param>
    /// <returns>The output streams.</returns>
    IObservable<TRecord>[] Split(IObservable<TRecord> stream, int count, IScheduler scheduler);
  }
}