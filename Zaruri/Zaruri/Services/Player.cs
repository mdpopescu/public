using System.Linq;
using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class Player : IPlayer
    {
        public Player(IRoller roller, IIndicesReader reader, IWriter writer)
        {
            this.roller = roller;
            this.reader = reader;
            this.writer = writer;

            amount = Constants.INITIAL_AMOUNT;
        }

        public bool HasMoney() => amount > 0;

        public void MakeBet()
        {
            amount -= 1;
            writer.WriteLine($"1$ bet; current amount: {amount}$");
        }

        public void InitialRoll()
        {
            roll = roller.GenerateDice().ToArray();
            ShowRoll("Initial roll");
        }

        public void FinalRoll()
        {
            var indices = reader.ReadIndices();
            roll = roll.Select((value, i) => indices.Contains(i + 1) ? value : roller.GenerateDie()).ToArray();
            ShowRoll("Final roll");
        }

        public void ComputeHand()
        {
            var hand = HANDS.Where(it => it.IsMatch(roll)).First();
            amount += hand.Score;
            writer.WriteLine($"{hand.Name}: {hand.Score}$ -- amount {amount}$");
        }

        //

        private static readonly Hand[] HANDS =
        {
            new HighFlush(),
            new LowFlush(),
            new FiveOfAKind(),
            new FourOfAKind(),
            new FullHouse(),
            new ThreeOfAKind(),
            new TwoPairs(),
            new OnePair(),
            new Nothing(),
        };

        private readonly IRoller roller;
        private readonly IIndicesReader reader;
        private readonly IWriter writer;

        private int amount;
        private int[] roll;

        private void ShowRoll(string prefix) => writer.WriteLine(prefix + $": {string.Join(" ", roll)}");
    }
}