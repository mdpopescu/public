using System;
using System.Drawing;
using System.IO;
using FindDuplicates.Contracts;

namespace FindDuplicates.Services
{
  public class ImageCache : Cache<string, Bitmap>
  {
    public ImageCache(ImageWriter writer, string rootFolder)
    {
      this.writer = writer;
      this.rootFolder = rootFolder;

      // ensure that the root folder exists
      Directory.CreateDirectory(rootFolder);
    }

    public Bitmap Get(string key, Func<string, Bitmap> func)
    {
      var path = Path.Combine(rootFolder, key + ".bmp");

      if (File.Exists(path))
        return (Bitmap) Image.FromFile(path);

      var image = func(key);
      writer.Save(image, path);

      return image;
    }

    //

    private readonly ImageWriter writer;
    private readonly string rootFolder;
  }
}