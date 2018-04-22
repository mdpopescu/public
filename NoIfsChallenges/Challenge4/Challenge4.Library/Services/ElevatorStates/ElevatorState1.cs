using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState1 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState1(string screen)
        {
            Info = new ElevatorInfo();

            Info.Floor3.Button1Enabled = true;
            Info.Floor3.Button2Enabled = false;
            Info.Floor3.Button3Enabled = false;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;

            Info.Floor2.Button1Enabled = true;
            Info.Floor2.Button2Enabled = false;
            Info.Floor2.Button3Enabled = false;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;

            Info.Floor1.Button1Enabled = false;
            Info.Floor1.Button2Enabled = true;
            Info.Floor1.Button3Enabled = true;
            Info.Floor1.Screen = screen;
        }

        public ElevatorState GoTo1st()
        {
            return this;
        }

        public ElevatorState GoTo2nd()
        {
            return new ElevatorState2(Constants.ELEVATOR_ARRIVES);
        }

        public ElevatorState GoTo3rd()
        {
            return new ElevatorState3(Constants.ELEVATOR_ARRIVES);
        }

        public ElevatorState CallTo1st()
        {
            return this;
        }

        public ElevatorState CallTo2nd()
        {
            return new ElevatorState2(Constants.ELEVATOR_CALLED);
        }

        public ElevatorState CallTo3rd()
        {
            return new ElevatorState3(Constants.ELEVATOR_CALLED);
        }
    }
}