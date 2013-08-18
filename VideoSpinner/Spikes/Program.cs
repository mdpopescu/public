using System;
using System.Linq;
using Renfield.VideoSpinner.Library;

namespace Renfield.Spikes
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var shuffler = new RandomShuffler();

            var list = Enumerable.Range(1, 10).ToList();
            var shuffled = shuffler.Shuffle(list);
            Console.WriteLine(string.Join(",", shuffled.Select(it => it.ToString())));

            const double DURATION = 100.0;
            const int COUNT = 20;
            var avg = DURATION / COUNT;
            Console.WriteLine("Avg: {0:0.00}, should be {1:0.00} - {2:0.00}", avg, avg - avg / 2, avg + avg / 2);

            var durations = shuffler.GetRandomizedDurations(DURATION, COUNT).ToList();
            Console.WriteLine(string.Join(",", durations.Select(it => it.ToString("#.00"))));

            Console.WriteLine("Sum: " + durations.Sum());
        }
    }
}