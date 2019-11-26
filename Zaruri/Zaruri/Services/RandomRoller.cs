using System;
using System.Collections.Generic;
using System.Linq;
using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class RandomRoller : IRoller
    {
        public IEnumerable<int> GenerateDice() => Enumerable.Range(1, Constants.DICE_COUNT).Select(_ => GenerateDie()).ToArray();

        public int GenerateDie() => RND.Next(1, Constants.DIE_MAX + 1);

        //

        private static readonly Random RND = new Random();
    }
}