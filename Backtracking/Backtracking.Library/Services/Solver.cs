using System;
using System.Collections.Generic;
using System.Linq;
using Backtracking.Library.Contracts;

namespace Backtracking.Library.Services
{
    public class Solver
    {
        public List<IState> Solve(IState initial)
        {
            var list = new List<IState> { initial };
            return GetSolution(list);
        }

        //

        private static List<IState> GetSolution(List<IState> list)
        {
            var current = list.Last();
            if (current.IsSolution(list))
                return list;
            if (current.IsInvalid(list))
                return null;

            Console.Error.Write($"{list.Count:D3} ");
            return current
                .GenerateCandidates()
                //.AsParallel()
                //.WithDegreeOfParallelism(32)
                .Select(candidate => GetSolution(list.Concat(new[] { candidate }).ToList()))
                .Where(it => it != null)
                .FirstOrDefault();

            //foreach (var candidate in candidates)
            //{
            //    list.Add(candidate);
            //    //Console.Error.WriteLine($"Attempting {candidate}");

            //    var solution = GetSolution(list);
            //    if (solution != null)
            //        return solution;

            //    list.Remove(candidate);
            //}

            //// none of the candidates were good
            //return null;
        }
    }
}