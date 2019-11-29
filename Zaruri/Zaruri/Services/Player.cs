using System.Linq;
using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class Player : IPlayer
    {
        // ReSharper disable once TooManyDependencies
        public Player(IRoller roller, IIndicesReader reader, IWriter writer, IPlayerLogic logic)
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
            roll = logic.FinalRoll(roll, reader.ReadIndices(), roller.GenerateDie).Unwrap(writer);
        }

        public void ComputeHand()
        {
            amount = logic.ComputeHand(roll, amount).Unwrap(writer);
        }

        //

        private readonly IRoller roller;
        private readonly IIndicesReader reader;
        private readonly IWriter writer;
        private readonly IPlayerLogic logic;

        private int amount;
        private int[] roll;
    }
}