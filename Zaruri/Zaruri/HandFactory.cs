using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaruri
{
    public class HandFactory
    {
        public Hand Create(int[] roll)
        {
            roll = InAscendingOrder(roll);
            var groupCounts = GetGroupCounts(roll);

            return CreateFromRoll(roll)
                ?? CreateFromGroupCounts(groupCounts)
                ?? new Nothing();
        }

        //

        private static readonly List<(int[], Type)> ROLL_MAP = new List<(int[], Type)>
        {
            (new[] { 2, 3, 4, 5, 6 }, typeof(HighFlush)),
            (new[] { 1, 2, 3, 4, 5 }, typeof(LowFlush)),
        };

        private static readonly List<(int[], Type)> GROUP_COUNTS_MAP = new List<(int[], Type)>
        {
            (new[] { 5 }, typeof(FiveOfAKind)),
            (new[] { 4, 1 }, typeof(FourOfAKind)),
            (new[] { 3, 2 }, typeof(FullHouse)),
            (new[] { 3, 1, 1 }, typeof(ThreeOfAKind)),
            (new[] { 2, 2, 1 }, typeof(TwoPairs)),
            (new[] { 2, 1, 1, 1 }, typeof(OnePair)),
        };

        private static int[] InAscendingOrder(IEnumerable<int> roll) => roll.OrderBy(it => it).ToArray();
        private static int[] GetGroupCounts(IEnumerable<int> roll) => roll.GroupBy(it => it).Select(it => it.Count()).OrderByDescending(it => it).ToArray();

        private static Hand CreateFromRoll(int[] roll) => CreateFromMap(ROLL_MAP, roll);
        private static Hand CreateFromGroupCounts(int[] groupCounts) => CreateFromMap(GROUP_COUNTS_MAP, groupCounts);

        private static Hand CreateFromMap(IEnumerable<(int[], Type)> map, int[] sequence)
        {
            return map
                .Where(it => it.Item1.SequenceEqual(sequence))
                .Select(it => it.Item2)
                .Select(type => (Hand) Activator.CreateInstance(type))
                .FirstOrDefault();
        }
    }
}