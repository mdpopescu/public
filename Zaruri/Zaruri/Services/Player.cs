using System;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Services
{
    public class Player : IPlayer
    {
        // ReSharper disable once TooManyDependencies
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
            amount = logic.MakeBet(amount).Unwrap(writer);
        }

        public void InitialRoll()
        {
            roll = logic.InitialRoll(roller.GenerateDice().ToArray()).Unwrap(writer);
        }

        public void FinalRoll()
        {
            roll = logic.FinalRoll(roll, ReadIndices(), roller.GenerateDie).Unwrap(writer);
        }

        public void ComputeHand()
        {
            amount = logic.ComputeHand(GetHand(), amount).Unwrap(writer);
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
                try
                {
                    return InternalReadIndices();
                }
                catch
                {
                    // try again
                }
        }

        private Indices InternalReadIndices()
        {
            writer.Write("Enter the dice to keep (1 .. 5), separated with spaces: ");
            var line = reader.ReadLine();
            var values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Select(Index.Create).ToArray();
            return Indices.Create(values);
        }

        private Hand GetHand() => HANDS.Where(it => it.IsMatch(roll)).First();
    }
}