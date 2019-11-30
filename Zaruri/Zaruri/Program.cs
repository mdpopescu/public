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
            var game = new Game(player);

            while (game.CanContinue())
            {
                game.Round();

                Console.WriteLine();
            }
        }
    }
}