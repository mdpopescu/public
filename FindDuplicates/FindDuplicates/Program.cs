using System;
using System.Collections.Generic;
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
    private const int THRESHOLD = 1;

    private static Cache<string, Bitmap> cache;
    private static ImageProcessor processor;

    private static void Main(string[] args)
    {
      var folder = args.Length > 0 ? args[0] : Environment.CurrentDirectory;

      // if there are any duplicates already identified, move everything back to the main folder first
      var dupsFolder = Path.Combine(folder, "dups");
      var dups = Directory.GetFiles(dupsFolder, "*.*", SearchOption.AllDirectories);
      foreach (var duplicate in dups)
      {
        TryMoveToFolder(duplicate, folder);
      }

      // if all files have been successfully moved, delete the dups folder
      dups = Directory.GetFiles(dupsFolder, "*.*", SearchOption.AllDirectories);
      if (!dups.Any())
        TryDeleteFolder(dupsFolder);

      var rootFolder = Path.Combine(folder, @"cache\");
      cache = new ImageCache2(rootFolder, new TextFileIndex(Path.Combine(rootFolder, "thumb.index")));
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

      var similars = GetSimilars(minified);
      var i = 0;
      foreach (var item in similars)
      {
        dupsFolder = Path.Combine(folder, "dups", i.ToString());
        Directory.CreateDirectory(dupsFolder);

        logger.WriteLine(item.FileName);
        TryMoveToFolder(item.FileName, dupsFolder);

        foreach (var similar in item.List)
        {
          logger.WriteLine(string.Format("  {0} = {1}", similar.FileName, similar.Distance));
          TryMoveToFolder(similar.FileName, dupsFolder);
        }

        i++;
      }

      logger.WriteLine("Total number of similar groups: " + i);
    }

    private static IEnumerable<Similars> GetSimilars(IReadOnlyList<Minified> minified)
    {
      var flags = new bool[minified.Count];
      for (var i = 0; i < minified.Count - 1; i++)
      {
        if (flags[i])
          continue;

        var x = minified[i];
        var similars = new Similars(x.FileName);
        for (var j = i + 1; j < minified.Count; j++)
        {
          var y = minified[j];
          var d = GetDistance(x.Hash, y.Hash);

          if (d <= THRESHOLD)
          {
            similars.List.Add(new Similar(y.FileName, d));
          }
        }

        if (similars.List.Any())
          yield return similars;
      }
    }

    private static Minified CreateMinified(string fileName)
    {
      try
      {
        var key = Path.GetFileNameWithoutExtension(fileName);

        var minified = cache.Get(key, _ => ActualMinify(fileName));
        var hash = processor.GetDiffHash(minified);

        return new Minified(fileName, minified, hash);
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

    private static void TryMoveToFolder(string source, string destFolder)
    {
      try
      {
        // ReSharper disable once AssignNullToNotNullAttribute
        File.Move(source, Path.Combine(destFolder, Path.GetFileName(source)));
      }
      catch (IOException)
      {
        // do nothing
      }
    }

    private static void TryDeleteFolder(string path)
    {
      try
      {
        Directory.Delete(path, true);
      }
      catch (IOException)
      {
        // do nothing
      }
    }
  }
}