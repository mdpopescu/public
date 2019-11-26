using System.Collections.Generic;

namespace Zaruri.Contracts
{
    public interface IRoller
    {
        IEnumerable<int> GenerateDice();
        int GenerateDie();
    }
}