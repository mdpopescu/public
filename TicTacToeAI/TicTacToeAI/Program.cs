using System;
using System.Linq;
using TicTacToeAI.Implementations;

namespace TicTacToeAI
{
    internal class Program
    {
        private static void Main()
        {
            var network = new Network(9, 9, 9, 9);
            network.Randomize();

            var outputs = network.Compute(0, 0, 0, 0, 1, 0, 0, 0, 0);
            Console.WriteLine(string.Join(" ", outputs.Select(v => v.ToString("F2"))));
        }
    }
}