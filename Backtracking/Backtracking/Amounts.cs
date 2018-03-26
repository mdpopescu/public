using System.Collections.Generic;
using System.Linq;
using Backtracking.Library.Contracts;

namespace Backtracking
{
    internal class Amounts : IState
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