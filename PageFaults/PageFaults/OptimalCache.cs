using System.Linq;

namespace Renfield.PageFaults
{
  public class OptimalCache : CacheBase
  {
    public int[] Future
    {
      get { return future; }
      set
      {
        future = value;
        futureIndex = 0;
      }
    }

    public OptimalCache(int cacheSize)
      : base(cacheSize)
    {
    }

    public override void AddPage(int pageNo)
    {
      if (!Pages.Contains(pageNo))
      {
        var index = GetFarthestUsedPageInTheFuture();
        Pages[index] = pageNo;
        PageFaults++;
      }

      futureIndex++;
    }

    //

    private int[] future;
    private int futureIndex;

    private int GetFarthestUsedPageInTheFuture()
    {
      var remainingFuture = future.Skip(futureIndex).ToList();
      var firstReuseForPage = Pages
        .Select(it => new { Page = it, Index = it.HasValue ? remainingFuture.IndexOf(it.Value) : int.MaxValue })
        .Select(it => new { it.Page, Index = it.Index == -1 ? int.MaxValue : it.Index });

      return firstReuseForPage
        .OrderByDescending(it => it.Index)
        .Select(it => it.Page)
        .Select(it => Pages.ToList().IndexOf(it))
        .First();
    }
  }
}