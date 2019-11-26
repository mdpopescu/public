using System;
using Zaruri.Services;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            var roller = new RandomRoller();
            var factory = new HandFactory();

            var player = new Player(roller, factory, 100);

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