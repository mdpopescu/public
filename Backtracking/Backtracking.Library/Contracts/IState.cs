using System.Collections.Generic;

namespace Backtracking.Library.Contracts
{
    public interface IState
    {
        bool IsSolution(List<IState> history);
        bool IsInvalid(List<IState> history);

        IEnumerable<IState> GenerateCandidates();
    }
}