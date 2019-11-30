using System;
using Zaruri.Services;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            var roller = new RandomRoller();
            var writer = new ConsoleWriter();
            var reader = new IndicesReader(writer, new ConsoleReader());

            var player = new Player(roller, reader, writer);

            while (player.HasMoney())
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