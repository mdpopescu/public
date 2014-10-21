using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using FindDuplicates.Models;
using FindDuplicates.Properties;
using FindDuplicates.Services;

namespace FindDuplicates
{
  internal class Program
  {
    private static FileCache cache;
    private static ImageProcessor processor;

    private static void Main(string[] args)
    {
      var folder = args.Length > 0 ? args[0] : Environment.CurrentDirectory;

      using (var writer = new BackgroundImageWriter())
      {
        cache = new FileCache(writer, Path.Combine(folder, @"cache\"));
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

        logger.WriteLine("Processing " + files.Count() + " files.");

        var minified = files
          .Select(CreateMinified)
          .Where(it => it != null)
          .ToList();

        logger.WriteLine("Minified.");

        var comparer = new ByteArrayComparer();
        var duplicates = minified
          .GroupBy(it => it.Hash, comparer)
          .Where(g => g.Count() > 1)
          .Select(g => string.Join(", ", g.Select(it => it.FileName)))
          .ToList();

        logger.WriteLine("Grouped.");

        foreach (var duplicate in duplicates)
        {
          logger.WriteLine(duplicate);
        }

        logger.WriteLine("Calculating distances...");

        var distances = (from x in minified
                         from y in minified
                         where string.Compare(x.FileName, y.FileName, StringComparison.InvariantCultureIgnoreCase) < 0
                         select new { x = x.FileName, y = y.FileName, d = GetDistance(x.Bytes, y.Bytes) })
          .ToList();

        logger.WriteLine("done.");

        foreach (var distance in distances)
        {
          Console.WriteLine("{0} .. {1} = {2}", distance.x, distance.y, distance.d);
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
        var hash = bytes.GetHash();

        return new Minified(fileName, minified, bytes, hash);
      }
      catch (OutOfMemoryException)
      {
        return null;
      }
    }

    private static Image ActualMinify(string fileName)
    {
      using (var original = Image.FromFile(fileName))
      {
        return processor.Minify(original);
      }
    }

    private static int GetDistance(byte[] x, byte[] y)
    {
      Debug.Assert(x.Length == y.Length);

      unchecked
      {
        var sum = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < x.Length; i++)
        {
          sum += Math.Abs(x[i] - y[i]);
        }

        return sum;
      }
    }
  }
}