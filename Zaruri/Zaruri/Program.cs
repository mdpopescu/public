using System;
using Zaruri.Services;

namespace Zaruri
{
    internal class Program
    {
        private static void Main()
        {
            var roller = new RandomRoller();
            var reader = new ConsoleReader();
            var writer = new ConsoleWriter();

            var player = new Player(roller, reader, writer, 100);
            var game = new Game(player);

            while (!game.IsOver())
            {
                game.Round();

                Console.WriteLine();
            }
        }
    }
}