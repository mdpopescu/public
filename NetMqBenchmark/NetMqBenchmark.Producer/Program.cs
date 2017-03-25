using NetMQ.ReactiveExtensions;

namespace NetMqBenchmark.Producer
{
    internal class Program
    {
        private static void Main()
        {
            var publisher = new PublisherNetMq<int>("tcp://127.0.0.1:56001");

            while (true)
                publisher.OnNext(42); // Sends 42.

            // ReSharper disable once FunctionNeverReturns
        }
    }
}