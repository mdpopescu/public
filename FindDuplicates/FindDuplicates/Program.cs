﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using FindDuplicates.Contracts;
using FindDuplicates.Models;
using FindDuplicates.Properties;
using FindDuplicates.Services;

namespace FindDuplicates
{
  internal class Program
  {
    private static Cache<string, Bitmap> cache;
    private static ImageProcessor processor;

    private static void Main(string[] args)
    {
      var folder = args.Length > 0 ? args[0] : Environment.CurrentDirectory;

      using (var writer = new BackgroundImageWriter())
      {
        //cache = new ImageCache(writer, Path.Combine(folder, @"cache\"));
        var rootFolder = Path.Combine(folder, @"cache\");
        cache = new ImageCache2(rootFolder, new TextFileIndex(Path.Combine(rootFolder, "thumb.index")));
        //cache = new NullImageCache();
        processor = new ImageProcessor();

        var logger = new ConsoleLogger();

        var extensions = Settings
          .Default
          .ImageExtensions
          .Cast<string>()
          .Select(it => it.Trim())
          .Where(it => !string.IsNullOrEmpty(it))
          .ToList();

        var files = Directory
          .GetFiles(folder)
          .Where(name => extensions.Any(name.EndsWith))
          .ToList();

        logger.WriteLine("Processing " + files.Count() + " files...");

        var minified = files
          .Select(CreateMinified)
          .Where(it => it != null)
          .ToList();

        logger.WriteLine("done.");

        logger.WriteLine("Looking for similar images...");

        var distances = (from x in minified
                         from y in minified
                         where string.Compare(x.FileName, y.FileName, StringComparison.InvariantCultureIgnoreCase) < 0
                         let d = GetDistance(x.Hash, y.Hash)
                         where d <= 10
                         select new { x = x.FileName, y = y.FileName, d })
          .OrderBy(it => it.d);

        foreach (var distance in distances)
        {
          logger.WriteLine(string.Format("{0} .. {1} = {2}", distance.x, distance.y, distance.d));
        }
      }
    }

    private static Minified CreateMinified(string fileName)
    {
      try
      {
        var key = Path.GetFileNameWithoutExtension(fileName);

        var minified = cache.Get(key, _ => ActualMinify(fileName));
        var bytes = processor.GetBytes(minified);
        var hash = processor.GetDiffHash(minified);

        return new Minified(fileName, minified, bytes, hash);
      }
      catch (OutOfMemoryException)
      {
        return null;
      }
    }

    private static Bitmap ActualMinify(string fileName)
    {
      using (var original = Image.FromFile(fileName))
      {
        return processor.MinifyAndGrayscale(original);
      }
    }

    private static int GetDistance(ulong x, ulong y)
    {
      unchecked
      {
        int result;

        var v = x ^ y;
        for (result = 0; v != 0; result++)
        {
          v &= v - 1;
        }

        return result;
      }
    }
  }
}