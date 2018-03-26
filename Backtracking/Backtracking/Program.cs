using System;
using System.IO;
using System.Linq;
using Backtracking.Lander.Models;
using Backtracking.Library.Services;

namespace Backtracking
{
    internal class Program
    {
        private static void Main()
        {
            //CoinsProblem();
            MarsLanderProblem();
        }

        //

        private static void CoinsProblem()
        {
            var solver = new Solver();

            do
            {
                // generate an amount as a sum of 3s and 5s
                Console.Write("Enter an amount: ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    return;

                var goal = int.Parse(input);
                var solution = solver.Solve(new Amounts(goal, 0));
                Console.WriteLine(solution == null ? "No solution." : string.Join("+", solution.Skip(1).Cast<Amounts>().Select(it => it.Current)));
            }
            while (true);
        }

        private static void MarsLanderProblem()
        {
            // ! IDEA: create a "fitness score" that is higher when the ship is within the landing zone (X, Y)
            // and lower when it's outside (the farther out, the lower)

            var solver = new Solver();

            var initial = new LanderState(new Position(2500, 2700), new Thrust(0, 3), new Speed(0, 0));
            var solution = solver.Solve(initial);

            if (solution == null)
                Console.WriteLine("No solution.");
            else
            {
                var thrusts = solution
                    .Cast<LanderState>()
                    .Select(it => it.ToString());
                File.WriteAllText(@"c:\temp\mars.txt", string.Join(Environment.NewLine, thrusts));
            }
        }
    }
}