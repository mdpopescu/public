using System;
using Zaruri.Contracts;
using Zaruri.Services;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            IRoller roller = new RandomRoller();
            IWriter writer = new ConsoleWriter();
            IIndicesReader reader = new IndicesReader(writer, new ConsoleReader());

            IPlayer player = new Player(roller, reader, writer);

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