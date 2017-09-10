using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gma.QrCodeNet.Encoding;
using Ionic.Zip;

namespace Renfield.QRMaker
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            Console.WriteLine($"Generating {COUNT} images...");

            var jobs = GenerateJobs().ToList();

            var watch = new Stopwatch();
            watch.Start();

            GenerateImages(jobs, 4);
            CreateArchive(jobs, "test.zip");
            DeleteImages(jobs, 4);

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
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static void GenerateImages(IEnumerable<JobInfo> jobs, int maxCpuThreads)
        {
            var dict = new ConcurrentDictionary<int, Generator>();

            Parallel.ForEach(jobs,
                new ParallelOptions { MaxDegreeOfParallelism = maxCpuThreads },
                job =>
                {
                    var generator = GetCurrentGenerator(dict, Thread.CurrentThread.ManagedThreadId);
                    generator.Render(job.Data, job.ImageFile);
                });
        }

        private static Generator GetCurrentGenerator(ConcurrentDictionary<int, Generator> dict, int managedThreadId)
        {
            // ReSharper disable once InvertIf
            if (!dict.TryGetValue(managedThreadId, out var result))
            {
                result = new Generator(new QrEncoder());
                dict.AddOrUpdate(managedThreadId, result, (_, __) => result);
            }

            return result;
        }

        private static void CreateArchive(IEnumerable<JobInfo> jobs, string archiveFile)
        {
            File.Delete(archiveFile);

            var zipper = new ZipFile(archiveFile);
            zipper.AddFiles(jobs.Select(it => it.ImageFile));
            zipper.Save();
        }

        private static void DeleteImages(IEnumerable<JobInfo> jobs, int maxCpuThreads)
        {
            Parallel.ForEach(jobs,
                new ParallelOptions { MaxDegreeOfParallelism = maxCpuThreads },
                job => File.Delete(job.ImageFile));
        }
    }
}