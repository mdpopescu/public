using Challenge4.Library.Models;

namespace Challenge4.Library.Contracts
{
    public interface ElevatorState
    {
        ElevatorInfo Info { get; }

        void GoTo1st();
        void GoTo2nd();
        void GoTo3rd();

        void CallTo1st();
        void CallTo2nd();
        void CallTo3rd();
    }
}