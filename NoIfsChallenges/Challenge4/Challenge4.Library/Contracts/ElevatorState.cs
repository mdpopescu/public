using Challenge4.Library.Models;

namespace Challenge4.Library.Contracts
{
    public interface ElevatorState
    {
        ElevatorInfo Info { get; }

        ElevatorState GoTo1st();
        ElevatorState GoTo2nd();
        ElevatorState GoTo3rd();

        ElevatorState CallTo1st();
        ElevatorState CallTo2nd();
        ElevatorState CallTo3rd();
    }
}