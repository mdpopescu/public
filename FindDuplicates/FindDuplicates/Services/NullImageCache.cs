using System;
using System.Drawing;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class NullImageCache : Cache<string, Bitmap>
  {
    public Bitmap Get(string key, Func<string, Bitmap> func)
    {
      return func(key);
    }
  }
}