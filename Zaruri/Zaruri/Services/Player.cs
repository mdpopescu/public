using System;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Services
{
    public class Player : IPlayer
    {
        public Player(IRoller roller, IReader reader, IWriter writer, IPlayerLogic logic)
        {
            this.roller = roller;
            this.reader = reader;
            this.writer = writer;
            this.logic = logic;

            amount = Constants.INITIAL_AMOUNT;
        }

        public bool IsBroke() => logic.IsBroke(amount);

        public void MakeBet()
        {
            var result = logic.MakeBet(amount);

            amount = result.Value;
            writer.WriteLine(result.Output);
        }

        public void InitialRoll()
        {
            var result = logic.InitialRoll(roller.GenerateDice().ToArray());

            roll = result.Value;
            writer.WriteLine(result.Output);
        }

        public void FinalRoll()
        {
            var result = logic.FinalRoll(roll, ReadIndices(), roller.GenerateDie);

            roll = result.Value;
            writer.WriteLine(result.Output);
        }

        public void ComputeHand()
        {
            var result = logic.ComputeHand(HANDS.Where(it => it.IsMatch(roll)).First(), amount);

            amount = result.Value;
            writer.WriteLine(result.Output);
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
        private readonly IPlayerLogic logic;

        private int amount;
        private int[] roll;

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