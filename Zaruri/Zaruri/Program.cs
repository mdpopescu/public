using System;
using Zaruri.Core;
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
            var logic = new PlayerLogic();

            var player = new Player(roller, reader, writer, logic);
            var game = new Game(player);

            while (!game.IsOver())
            {
                game.Round();

                Console.WriteLine();
            }
        }
    }
}