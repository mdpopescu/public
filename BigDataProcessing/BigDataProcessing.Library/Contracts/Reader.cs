using System.Collections.Generic;

namespace BigDataProcessing.Library.Contracts
{
  public interface Reader<in TSource, out TRecord>
  {
    IEnumerable<TRecord> Read(TSource source);
  }
}