using System;
using System.Diagnostics;
using System.IO;
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
            Run(() => new MemoryStream(), true);
            Run(() => new MemoryStream(), false);

            Console.WriteLine("On-disk");
            Run(() => new FileStream(Path.GetTempFileName(), FileMode.Create), true);
            Run(() => new FileStream(Path.GetTempFileName(), FileMode.Create), false);
        }

        private static void Run(Func<Stream> streamFactory, bool autoFlush)
        {
            var buffer = new byte[100];
            using (var ms = streamFactory.Invoke())
            {
                var stream = new WORMStream(ms) { AutoFlush = autoFlush };
                var benchmark = Benchmark(stream, it => it.Append(buffer));
                var prefix = autoFlush ? "With" : "Without";
                WriteResult(benchmark, $"{prefix} auto-flush");
            }
        }

        private static TimeSpan Benchmark(WORMStream stream, Action<WORMStream> action)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < COUNT; i++)
                action(stream);
            stream.Flush();

            sw.Stop();
            return sw.Elapsed;
        }

        private static void WriteResult(TimeSpan benchmark, string prefix)
        {
            var wps = COUNT / benchmark.TotalSeconds;
            Console.WriteLine(prefix + $": {benchmark} ({wps:N2} writes per second)");
        }
    }
}