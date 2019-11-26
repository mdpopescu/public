namespace Zaruri
{
    public interface Hand
    {
        string Name { get; }
        int Score { get; }
    }

    public class HighFlush : Hand
    {
        public string Name => "High Flush";
        public int Score => 30;
    }

    public class LowFlush : Hand
    {
        public string Name => "Low Flush";
        public int Score => 25;
    }

    public class FiveOfAKind : Hand
    {
        public string Name => "Five of a Kind";
        public int Score => 20;
    }

    public class FourOfAKind : Hand
    {
        public string Name => "Four of a Kind";
        public int Score => 15;
    }

    public class FullHouse : Hand
    {
        public string Name => "Full House";
        public int Score => 10;
    }

    public class ThreeOfAKind : Hand
    {
        public string Name => "Three of a Kind";
        public int Score => 5;
    }

    public class TwoPairs : Hand
    {
        public string Name => "Two Pairs";
        public int Score => 3;
    }

    public class OnePair : Hand
    {
        public string Name => "One Pair";
        public int Score => 2;
    }

    public class Nothing : Hand
    {
        public string Name => "Nothing";
        public int Score => 0;
    }
}