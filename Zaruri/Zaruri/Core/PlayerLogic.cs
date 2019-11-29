using System;
using System.Collections.Generic;
using System.Linq;
using Zaruri.Contracts;
using Zaruri.Models;

namespace Zaruri.Core
{
    public class PlayerLogic : IPlayerLogic
    {
        public bool IsBroke(int amount) => amount <= 0;

        public (string, int) MakeBet(int amount)
        {
            var newAmount = amount - 1;
            return ($"1$ bet; current amount: {newAmount}$", newAmount);
        }

        public (string, int[]) InitialRoll(int[] roll)
        {
            return (ShowRoll("Initial roll", roll), roll);
        }

        public (string, int[]) FinalRoll(int[] roll, Indices indices, Func<int> rollDie)
        {
            var newRoll = roll.Select((value, i) => indices.Contains(i + 1) ? value : rollDie()).ToArray();
            return (ShowRoll("Final roll", newRoll), newRoll);
        }

        public (string, int) ComputeHand(Hand hand, int amount)
        {
            var newAmount = amount + hand.Score;
            return ($"{hand.Name}: {hand.Score}$ -- amount {newAmount}$", newAmount);
        }

        //

        private static string ShowRoll(string prefix, IEnumerable<int> roll) => prefix + $": {string.Join(" ", roll)}";
    }
}