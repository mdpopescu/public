using System.Linq;

namespace Renfield.PageFaults
{
  public class FifoCache : CacheBase
  {
    public FifoCache(int cacheSize)
      : base(cacheSize)
    {
    }

    public override void AddPage(int pageNo)
    {
      if (Pages.Contains(pageNo))
        return;

      InsertInFront(pageNo);
      PageFaults++;
    }

    //

    private void InsertInFront(int pageNo)
    {
      for (var i = Pages.Length - 1; i > 0; i--)
        Pages[i] = Pages[i - 1];
      Pages[0] = pageNo;
    }
  }
}