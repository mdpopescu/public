using System;
using Zaruri.Services;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            var roller = new RandomRoller();

            var player = new Player(roller, 100);

            while (!player.IsBroke())
            {
                player.MakeBet();
                player.InitialRoll();
                player.FinalRoll();
                player.ComputeHand();

                Console.WriteLine();
            }
        }
    }
}