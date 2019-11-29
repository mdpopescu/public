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
            string line;
            (line, amount) = logic.MakeBet(amount);

            writer.WriteLine(line);
        }

        public void InitialRoll()
        {
            string line;
            (line, roll) = logic.InitialRoll(roller.GenerateDice().ToArray());

            writer.WriteLine(line);
        }

        public void FinalRoll()
        {
            string line;
            (line, roll) = logic.FinalRoll(roll, ReadIndices(), roller.GenerateDie);

            writer.WriteLine(line);
        }

        public void ComputeHand()
        {
            string line;
            (line, amount) = logic.ComputeHand(HANDS.Where(it => it.IsMatch(roll)).First(), amount);

            writer.WriteLine(line);
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