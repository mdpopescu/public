using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState2 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState2(string screen)
        {
            Info = new ElevatorInfo();

            Info.Floor3.Button1Enabled = true;
            Info.Floor3.Button2Enabled = false;
            Info.Floor3.Button3Enabled = false;
            Info.Floor3.Screen = "Door Closed";

            Info.Floor2.Button1Enabled = false;
            Info.Floor2.Button2Enabled = true;
            Info.Floor2.Button3Enabled = true;
            Info.Floor2.Screen = screen;

            Info.Floor1.Button1Enabled = true;
            Info.Floor1.Button2Enabled = false;
            Info.Floor1.Button3Enabled = false;
            Info.Floor1.Screen = "Door Closed";
        }

        public ElevatorState GoTo1st()
        {
            return new ElevatorState1("Elevator Arrives - Door Opens - User Exits");
        }

        public ElevatorState GoTo2nd()
        {
            return this;
        }

        public ElevatorState GoTo3rd()
        {
            return new ElevatorState3("Elevator Arrives - Door Opens - User Exits");
        }

        public ElevatorState CallTo1st()
        {
            return new ElevatorState1("Elevator Called Up - Door Opens");
        }

        public ElevatorState CallTo2nd()
        {
            return this;
        }

        public ElevatorState CallTo3rd()
        {
            return new ElevatorState3("Elevator Called Up - Door Opens");
        }
    }
}