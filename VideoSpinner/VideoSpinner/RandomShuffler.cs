using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.VideoSpinner
{
    public class RandomShuffler : Shuffler
    {
        public RandomShuffler(Random rnd)
        {
            this.rnd = rnd;
        }

        public IList<T> Shuffle<T>(IList<T> list)
        {
            // generate a list of N random numbers
            // add the last part of the item hash to avoid collisions
            var order = Enumerable
                .Range(1, list.Count)
                .Select(_ => rnd.Next(list.Count))
                .Select((i, r) => r << 8 + list[i].GetHashCode() & 0xFF)
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