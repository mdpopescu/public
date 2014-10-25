using System;

namespace FindDuplicates.Contracts
{
  // Item1 = position, Item2 = size
  public interface Index
  {
    Tuple<long, long> Get(string key);
    void Set(string key, Tuple<long, long> tuple);
  }
}