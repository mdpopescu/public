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

        /// <summary>
        /// Gets a randomized list of durations that sum to the given total duration
        /// </summary>
        /// <param name="duration">Total duration</param>
        /// <param name="count">Number of values</param>
        /// <returns>Sequence of randomized durations that sum to the given duration</returns>
        public IEnumerable<double> GetRandomizedDurations(double duration, int count)
        {
            Contract.Requires(duration > 0.0);
            Contract.Requires(count >= 1);

            // The algorithm
            // =============
            // Imagine the durations as the dashes in this graphic:
            // |--|--|--|--|--| <- 5 intervals, 6 "fenceposts"
            // Instead of trying to come up with a sequence of values for the durations that are 1) randomized and
            // 2) nevertheless sum to the given value, I will simply move the "fenceposts" to the left or to the right
            // for a distance <= d / 4, where d is the original interval size; the first and last fenceposts
            // will stay in the same places.
            // For example, this would be a valid result:
            // |-|--|---|---|-|

            var indices = Enumerable.Range(0, count + 1).ToList(); // count + 1 fenceposts

            var avg = duration / count;
            var startTimes = indices.Select(i => i * avg).ToList();

            var rnd = new Random();
            var jitters = indices.Select(i => rnd.NextDouble() * (avg / 2) - (avg / 4)).ToList();

            // move the fenceposts
            var results = startTimes.Zip(jitters, (s, j) => s + j).ToList();

            // restore the first and last fenceposts
            results[0] = 0;
            results[count] = startTimes[count];

            // return the intervals between fenceposts
            for (var i = 1; i <= count; i++)
                yield return results[i] - results[i - 1];
        }
    }
}