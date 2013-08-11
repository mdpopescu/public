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
            for (var i = 1; i <= 5; i++)
            {
                var shuffled = shuffler.Shuffle(list);
                Console.WriteLine(string.Join(",", shuffled.Select(it => it.ToString())));
            }
        }
    }
}