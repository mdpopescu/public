using System;

namespace Renfield.PageFaults
{
  public abstract class CacheBase : Cache
  {
    public int?[] Pages { get; private set; }
    public int PageFaults { get; protected set; }

    protected CacheBase(int count)
    {
      if (count < 1)
        throw new ArgumentOutOfRangeException("count");

      Pages = new int?[count];
      PageFaults = 0;
    }

    public abstract void AddPage(int pageNo);
  }
}