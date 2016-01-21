using System;
using System.Collections;

namespace BigDataProcessing.Library.Contracts
{
  public interface RxSplitter<out TRecord>
  {
    /// <summary>
    ///   Splits the input sequence into <see cref="count" /> concurrent output streams.
    /// </summary>
    /// <param name="enumerator">The enumerator for the input sequence.</param>
    /// <param name="count">The number of output streams.</param>
    /// <returns>The output streams.</returns>
    IObservable<TRecord>[] Split(IEnumerator enumerator, int count);
  }
}