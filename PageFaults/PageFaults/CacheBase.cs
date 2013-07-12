using System;

namespace Renfield.PageFaults
{
  public abstract class CacheBase : Cache
  {
    public int CacheSize { get; private set; }
    public int?[] Pages { get; private set; }
    public int PageFaults { get; protected set; }

    protected CacheBase(int cacheSize)
    {
      if (cacheSize < 1)
        throw new ArgumentOutOfRangeException("cacheSize");

      CacheSize = cacheSize;
      Pages = new int?[cacheSize];
      PageFaults = 0;
    }

    public abstract void AddPage(int pageNo);
  }
}