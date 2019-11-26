using System.Linq;

namespace Zaruri
{
    public class HandFactory
    {
        public Hand Create(int[] roll)
        {
            roll = roll.OrderBy(it => it).ToArray();

            var groups = roll.GroupBy(it => it).ToArray();
            var groupCounts = groups.Select(it => it.Count()).OrderByDescending(it => it).ToArray();

            if (roll.SequenceEqual(new[] { 2, 3, 4, 5, 6 }))
                return new HighFlush();
            if (roll.SequenceEqual(new[] { 1, 2, 3, 4, 5 }))
                return new LowFlush();

            if (groupCounts.SequenceEqual(new[] { 5 }))
                return new FiveOfAKind();
            if (groupCounts.SequenceEqual(new[] { 4, 1 }))
                return new FourOfAKind();
            if (groupCounts.SequenceEqual(new[] { 3, 2 }))
                return new FullHouse();
            if (groupCounts.SequenceEqual(new[] { 3, 1, 1 }))
                return new ThreeOfAKind();
            if (groupCounts.SequenceEqual(new[] { 2, 2, 1 }))
                return new TwoPairs();
            if (groupCounts.SequenceEqual(new[] { 2, 1, 1, 1 }))
                return new OnePair();

            return new Nothing();
        }
    }
}