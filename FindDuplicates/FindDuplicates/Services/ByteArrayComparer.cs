using System;
using System.Collections.Generic;

namespace FindDuplicates.Services
{
  public class ByteArrayComparer : IEqualityComparer<byte[]>
  {
    public bool Equals(byte[] x, byte[] y)
    {
      return x == y || (x.Length == y.Length && NativeMethods.memcmp(x, y, new UIntPtr((uint) x.Length)) == 0);
    }

    public int GetHashCode(byte[] obj)
    {
      unchecked
      {
        var result = 0;
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var b in obj)
          result = (result * 31) ^ b;

        return result;
      }
    }
  }
}