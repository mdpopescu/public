using System;
using System.Linq;
using CachingAlgorithms.Implementations;
using CachingAlgorithms.Interfaces;

namespace CachingAlgorithms
{
    class Program
    {
        private const int CAPACITY = 5; // cache capacity
        private const int MAX = 20; // total number of keys
        private const int RUNS = 1000; // total number of cache accesses

        private const int SEED = 1;

        private static void Main(string[] args)
        {
            ReportMisses("PerfectCache", new PerfectCache<int, string>(CAPACITY, GetRuns(RUNS)));
            ReportMisses("LeastFutureUse", new LeastFutureUse<int, string>(CAPACITY, GetRuns(RUNS)));
            ReportMisses("LeastRecentlyUsed", new LeastRecentlyUsed<int, string>(CAPACITY));
            ReportMisses("MostRecentlyUsed", new MostRecentlyUsed<int, string>(CAPACITY));
        }

        private static void ReportMisses(string prefix, Cache<int, string> cache)
        {
            var misses = GetMisses(cache);
            Console.WriteLine(prefix + " -- Total number of misses: " + misses);
        }

        private static int GetMisses(Cache<int, string> cache)
        {
            var rnd = new Random(SEED);

            var misses = 0;
            Func<int, string> getValue = _ =>
            {
                misses++;
                return "";
            };

            for (var i = 0; i < RUNS; i++)
                cache.Get(GetRandomValue(rnd), getValue);

            return misses;
        }

        private static int[] GetRuns(int count)
        {
            var rnd = new Random(SEED);

            return Enumerable
                .Range(1, count)
                .Select(_ => GetRandomValue(rnd))
                .ToArray();
        }

        private static int GetRandomValue(Random rnd)
        {
            return rnd.Next(MAX);
        }
    }
}