using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Renfield.VideoSpinner.Library
{
    public class RandomShuffler : Shuffler
    {
        public IList<T> Shuffle<T>(IList<T> list)
        {
            Contract.Requires(list.Any());

            // generate a list of GUIDs
            var N = list.Count;
            var order = Enumerable
                .Range(1, N)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();

            // return the original list ordered by the random ranks
            return list
                .Zip(order, (it, rank) => new {it, rank})
                .OrderBy(pair => pair.rank)
                .Select(pair => pair.it)
                .ToList();
        }
    }
}