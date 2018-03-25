using System;
using System.Collections.Generic;
using System.Linq;
using Backtracking.Library.Contracts;
using Backtracking.Library.Services;

namespace Backtracking
{
    internal class Program
    {
        private static void Main()
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

        //

        private class Amounts : IState
        {
            public int Current { get; }

            public Amounts(int goal, int current)
            {
                this.goal = goal;
                Current = current;
            }

            public bool IsSolution(List<IState> history)
            {
                return history.Cast<Amounts>().Select(it => it.Current).Sum() == goal;
            }

            public bool IsInvalid(List<IState> history)
            {
                return history.Cast<Amounts>().Select(it => it.Current).Sum() > goal;
            }

            public IEnumerable<IState> GenerateCandidates()
            {
                return new[] { new Amounts(goal, 3), new Amounts(goal, 5), };
            }

            //

            private readonly int goal;
        }
    }
}