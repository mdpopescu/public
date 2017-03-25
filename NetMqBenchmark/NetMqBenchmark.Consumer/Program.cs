using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NetMQ.ReactiveExtensions;

namespace NetMqBenchmark.Consumer
{
    internal class Program
    {
        private static void Main()
        {
            var subscriber = new SubscriberNetMq<int>("tcp://127.0.0.1:56001");

            long count = 0;
            var sw = new Stopwatch();
            sw.Start();

            subscriber.Subscribe(_ => ProcessMessage(ref count, sw), () => sw.Stop());

            Console.ReadLine();
        }

        private static void ProcessMessage(ref long count, Stopwatch sw)
        {
            count++;
            if (count % 1000000 != 0)
                return;

            var l = count;
            //ReportStats(l, sw.Elapsed);
            Task.Run(() => ReportStats(l, sw.Elapsed));
        }

        private static void ReportStats(long count, TimeSpan elapsed)
        {
            Console.WriteLine($"Received {count} messages in {elapsed.TotalSeconds:F} seconds => {count / elapsed.TotalSeconds:F} msg/sec");
        }
    }
}