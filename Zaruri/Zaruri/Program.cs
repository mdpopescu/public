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
            var reader = new ConsoleReader();
            var writer = new ConsoleWriter();
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