using System;
using System.Linq;
using Renfield.VideoSpinner;

namespace Renfield.Spikes
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var shuffler = new RandomShuffler(new Random());

            var list = Enumerable.Range(1, 20).ToList();
            for (var i = 1; i <= 5; i++)
            {
                var shuffled = shuffler.Shuffle(list);
                Console.WriteLine(string.Join(",", shuffled.Select(it => it.ToString())));
            }
        }
    }
}