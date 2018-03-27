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

        //private readonly Dictionary<IState, List<IState>> cache = new Dictionary<IState, List<IState>>();

        private List<IState> GetSolution(List<IState> list)
        {
            var current = list.Last();

            //if (cache.ContainsKey(current))
            //    return cache[current];

            List<IState> result = null;

            if (current.IsSolution(list))
                result = list;
            else if (!current.IsInvalid(list))
            {
                Console.Error.Write($"{list.Count:D3} ");
                result = current
                    .GenerateCandidates()
                    //.AsParallel()
                    //.WithDegreeOfParallelism(32)
                    .Select(candidate => GetSolution(list.Concat(new[] { candidate }).ToList()))
                    .Where(it => it != null)
                    .FirstOrDefault();
            }

            //cache[current] = result;
            return result;
        }
    }
}