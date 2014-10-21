using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class BackgroundImageWriter : ImageWriter
  {
    public BackgroundImageWriter()
    {
      pending = new BlockingCollection<Tuple<Image, string>>();
      Task.Run(() => SaveImages());
    }

    public void Dispose()
    {
      pending.CompleteAdding();
    }

    public void Save(Image image, string path)
    {
      pending.Add(Tuple.Create(image, path));
    }

    //

    private readonly BlockingCollection<Tuple<Image, string>> pending;

    private void SaveImages()
    {
      foreach (var tuple in pending.GetConsumingEnumerable())
      {
        lock (tuple.Item1)
        {
          tuple.Item1.Save(tuple.Item2, ImageFormat.Bmp);
        }
      }
    }
  }
}