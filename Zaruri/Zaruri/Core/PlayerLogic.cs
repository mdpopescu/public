using System;
using System.Collections.Generic;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;
using Zaruri.Services;

namespace Zaruri.Core
{
    public class PlayerLogic : IPlayerLogic
    {
        public bool IsBroke(int amount) => amount <= 0;

        public OutputWrapper<int> MakeBet(int amount)
        {
            var newAmount = amount - 1;
            return newAmount.WithOutput($"1$ bet; current amount: {newAmount}$");
        }

        public OutputWrapper<int[]> InitialRoll(int[] roll) => roll.WithOutput(ShowRoll("Initial roll", roll));

        public OutputWrapper<int[]> FinalRoll(int[] roll, Indices indices, Func<int> rollDie)
        {
            var newRoll = roll.Select((value, i) => indices.Contains(i + 1) ? value : rollDie()).ToArray();
            return newRoll.WithOutput(ShowRoll("Final roll", newRoll));
        }

        public OutputWrapper<int> ComputeHand(Hand hand, int amount)
        {
            var newAmount = amount + hand.Score;
            return newAmount.WithOutput($"{hand.Name}: {hand.Score}$ -- amount {newAmount}$");
        }

        //

        private static string ShowRoll(string prefix, IEnumerable<int> roll) => prefix + $": {string.Join(" ", roll)}";
    }
}