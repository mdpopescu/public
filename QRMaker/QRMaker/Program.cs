using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gma.QrCodeNet.Encoding;
using Ionic.Zip;

namespace Renfield.QRMaker
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var jobs = GenerateJobs().ToList();

      var watch = new Stopwatch();
      watch.Start();

      GenerateImages(jobs);
      CreateArchive(jobs, "test.zip");
      DeleteImages(jobs);

      watch.Stop();
      Console.WriteLine(watch.ElapsedMilliseconds + " msec elapsed.");
    }

    //

    private const int COUNT = 10000;

    /// <summary>
    ///   Generate test data
    /// </summary>
    private static IEnumerable<JobInfo> GenerateJobs()
    {
      for (var i = 0; i < COUNT; i++)
        yield return new JobInfo { Data = GetBytes("Some data " + i), ImageFile = i + ".png" };
    }

    private static byte[] GetBytes(string str)
    {
      var bytes = new byte[str.Length * sizeof (char)];
      Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
      return bytes;
    }

    private static void GenerateImages(IEnumerable<JobInfo> jobs)
    {
      var generator = new Generator(new QrEncoder());
      foreach (var job in jobs)
        generator.Render(job.Data, job.ImageFile);
    }

    private static void CreateArchive(IEnumerable<JobInfo> jobs, string archiveFile)
    {
      File.Delete(archiveFile);

      var zipper = new ZipFile(archiveFile);
      foreach (var job in jobs)
        zipper.AddFile(job.ImageFile);
      zipper.Save();
    }

    private static void DeleteImages(IEnumerable<JobInfo> jobs)
    {
      foreach (var job in jobs)
        File.Delete(job.ImageFile);
    }
  }
}