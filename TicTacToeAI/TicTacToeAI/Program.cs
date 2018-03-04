using System;
using System.Linq;
using TicTacToeAI.Extensions;
using TicTacToeAI.Implementations;

namespace TicTacToeAI
{
    internal class Program
    {
        private static void Main()
        {
            var solver = new Solver(() => new TicTacToeGame(1.0f, GetUserMove)) { OnGeneration = i => Console.WriteLine($"Generation {i}.") };
            solver.Solve(new Network(9, 9, 9, 9, 1), 10);
        }

        private static int GetUserMove(int[] available)
        {
            do
            {
                Console.Write($"Your move (valid choices: {string.Join(",", available)})? ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var choice) && available.Contains(choice))
                    return choice;
            }
            while (true);
        }
    }
}