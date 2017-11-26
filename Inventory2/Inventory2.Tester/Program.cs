using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Inventory2.Library.Shell;

namespace Inventory2.Tester
{
    internal class Program
    {
        private const int COUNT = 100000;

        private static void Main()
        {
            InMemorySpeedTest();
        }

        private static void InMemorySpeedTest()
        {
            Console.WriteLine("In-memory");
            Run(() => new MemoryStream(), COUNT * 10, true);
            Run(() => new MemoryStream(), COUNT * 10, false);

            Console.WriteLine("On-disk");
            Run(() => new FileStream(Path.GetTempFileName(), FileMode.Create), COUNT, true);
            Run(() => new FileStream(Path.GetTempFileName(), FileMode.Create), COUNT, false);
        }

        private static void Run(Func<Stream> streamFactory, int count, bool autoFlush)
        {
            var buffer = new byte[100];
            using (var ms = streamFactory.Invoke())
            {
                var stream = new WORMStream(ms) { AutoFlush = autoFlush };
                var timeSpan = Benchmark(stream, count, it => it.Append(buffer));
                var prefix = autoFlush ? "With" : "Without";
                Console.WriteLine($"{prefix} auto-flush");
                WriteResult(timeSpan, "Writing");

                timeSpan = Benchmark(stream, 1, it => DoNothing(it.ReadAll()));
                WriteResult(timeSpan, "Reading");
            }
        }

        private static TimeSpan Benchmark(WORMStream stream, int count, Action<WORMStream> action)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < count; i++)
                action(stream);
            stream.Flush();

            sw.Stop();
            return sw.Elapsed;
        }

        private static void WriteResult(TimeSpan benchmark, string prefix)
        {
            var ops = COUNT / benchmark.TotalSeconds;
            Console.WriteLine($"{prefix}: {benchmark} ({ops:N0} operations per second)");
        }

        private static void DoNothing(IEnumerable<byte[]> readAll)
        {
            Console.WriteLine($"ReadAll returned {readAll.Count():N0} records.");
        }
    }
}