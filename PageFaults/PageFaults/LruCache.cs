using System;

namespace Renfield.PageFaults
{
  public class LruCache : CacheBase
  {
    public LruCache(int count)
      : base(count)
    {
    }

    public override void AddPage(int pageNo)
    {
      var index = Array.IndexOf(Pages, pageNo);
      if (index >= 0)
        MoveToFront(index);
      else
      {
        InsertInFront(pageNo);
        PageFaults++;
      }
    }

    //

    private void MoveToFront(int index)
    {
      var temp = Pages[index];
      for (var i = index; i > 0; i--)
        Pages[i] = Pages[i - 1];
      Pages[0] = temp;
    }

    private void InsertInFront(int pageNo)
    {
      for (var i = Pages.Length - 1; i > 0; i--)
        Pages[i] = Pages[i - 1];
      Pages[0] = pageNo;
    }
  }
}