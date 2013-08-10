using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Renfield.VideoSpinner.Library
{
    public class RandomShuffler : Shuffler
    {
        public RandomShuffler(Random rnd)
        {
            this.rnd = rnd;
        }

        public IList<T> Shuffle<T>(IList<T> list)
        {
            Contract.Requires(list.Any());
            Contract.Requires(list.Count < 65536);

            // generate a list of N random numbers
            // add the last part of the item hash to avoid collisions
            var N = list.Count;
            var N2 = N * N;
            var order = Enumerable
                .Range(1, N)
                .Select(_ => rnd.Next(N2))
                .Select((r, i) => (r << 8) | (list[i].GetHashCode() & 0xFF))
                .ToList();

            // return the original list ordered by the random ranks
            return list
                .Zip(order, (it, rank) => new {it, rank})
                .OrderBy(pair => pair.rank)
                .Select(pair => pair.it)
                .ToList();
        }

        //

        private readonly Random rnd;
    }
}