using System;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Services
{
    public class Player : IPlayer
    {
        public Player(IRoller roller, IReader reader, IWriter writer, int amount)
        {
            this.roller = roller;
            this.reader = reader;
            this.writer = writer;

            this.amount = amount;
        }

        public bool IsBroke() => amount <= 0;

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
            var indices = ReadIndices();

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
        private readonly IReader reader;
        private readonly IWriter writer;

        private int amount;
        private int[] roll;

        private void ShowRoll(string prefix) => writer.WriteLine(prefix + $": {string.Join(" ", roll)}");

        private Indices ReadIndices()
        {
            while (true)
            {
                writer.Write("Enter the dice to keep (1 .. 5), separated with spaces: ");
                try
                {
                    return Parse(reader.ReadLine());
                }
                catch
                {
                    // try again
                }
            }
        }

        private static Indices Parse(string line)
        {
            var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Select(Index.Create).ToArray();
            return Indices.Create(values);
        }
    }
}