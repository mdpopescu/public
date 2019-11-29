using System.Collections.Generic;
using System.Linq;

namespace Zaruri
{
    public interface Hand
    {
        string Name { get; }
        int Score { get; }

        bool IsMatch(int[] roll);
    }

    public abstract class HandBase : Hand
    {
        public abstract string Name { get; }
        public abstract int Score { get; }

        public virtual bool IsMatch(int[] roll)
        {
            roll = InAscendingOrder(roll);
            var groupCounts = GetGroupCounts(roll);

            return IsRollMatch(roll) || IsGroupMatch(groupCounts);
        }

        //

        protected virtual bool IsRollMatch(int[] roll) => false;
        protected virtual bool IsGroupMatch(int[] groupCounts) => false;

        //

        private static int[] InAscendingOrder(IEnumerable<int> roll) => roll.OrderBy(it => it).ToArray();
        private static int[] GetGroupCounts(IEnumerable<int> roll) => roll.GroupBy(it => it).Select(it => it.Count()).OrderByDescending(it => it).ToArray();
    }

    public class HighFlush : HandBase
    {
        public override string Name => "High Flush";
        public override int Score => 15;

        //

        protected override bool IsRollMatch(int[] roll) => roll.SequenceEqual(new[] { 2, 3, 4, 5, 6 });
    }

    public class LowFlush : HandBase
    {
        public override string Name => "Low Flush";
        public override int Score => 13;

        //

        protected override bool IsRollMatch(int[] roll) => roll.SequenceEqual(new[] { 1, 2, 3, 4, 5 });
    }

    public class FiveOfAKind : HandBase
    {
        public override string Name => "Five of a Kind";
        public override int Score => 10;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 5 });
    }

    public class FourOfAKind : HandBase
    {
        public override string Name => "Four of a Kind";
        public override int Score => 8;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 4, 1 });
    }

    public class FullHouse : HandBase
    {
        public override string Name => "Full House";
        public override int Score => 5;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 3, 2 });
    }

    public class ThreeOfAKind : HandBase
    {
        public override string Name => "Three of a Kind";
        public override int Score => 3;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 3, 1, 1 });
    }

    public class TwoPairs : HandBase
    {
        public override string Name => "Two Pairs";
        public override int Score => 2;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 2, 2, 1 });
    }

    public class OnePair : HandBase
    {
        public override string Name => "One Pair";
        public override int Score => 1;

        //

        protected override bool IsGroupMatch(int[] groupCounts) => groupCounts.SequenceEqual(new[] { 2, 1, 1, 1 });
    }

    public class Nothing : HandBase
    {
        public override string Name => "Nothing";
        public override int Score => 0;

        public override bool IsMatch(int[] roll) => true;
    }
}