using System;
using System.Drawing;
using System.IO;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class ImageCache2 : IDisposable, Cache<string, Bitmap>
  {
    public ImageCache2(string rootFolder, Index index)
    {
      this.index = index;

      data = new FileStream(Path.Combine(rootFolder, "thumb.data"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    public void Dispose()
    {
      data.Flush();
      data.Dispose();
    }

    public Bitmap Get(string key, Func<string, Bitmap> func)
    {
      var tuple = index.Get(key);
      if (tuple != null)
      {
        return ReadBitmap(tuple.Item1, tuple.Item2);
      }

      var bmp = func(key);
      WriteBitmap(key, bmp);

      return bmp;
    }

    //

    private readonly Index index;
    private readonly Stream data;

    private Bitmap ReadBitmap(long position, long size)
    {
      var buffer = new byte[size];
      data.Seek(position, SeekOrigin.Begin);
      data.Read(buffer, 0, buffer.Length);

      return buffer.ToBitmap();
    }

    private void WriteBitmap(string key, Bitmap bmp)
    {
      var buffer = bmp.ToByteArray();

      data.Seek(0, SeekOrigin.End);
      var position = data.Position;
      data.Write(buffer, 0, buffer.Length);
      data.Flush();

      var tuple = Tuple.Create(position, (long) buffer.Length);
      index.Set(key, tuple);
    }
  }
}