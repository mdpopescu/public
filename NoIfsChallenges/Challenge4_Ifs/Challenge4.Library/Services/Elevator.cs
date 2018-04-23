using Challenge4.Library.Models;

namespace Challenge4.Library.Services
{
    public class Elevator
    {
        public ElevatorInfo Info { get; }

        public Elevator()
        {
            Info = new ElevatorInfo();

            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;
            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;
            Info.Floor1.CallEnabled = false;
            Info.Floor1.Screen = Constants.DOOR_OPEN;

            floor = 1;
        }

        public void GoTo1st()
        {
            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;
            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;
            Info.Floor1.CallEnabled = false;
            Info.Floor1.Screen = Constants.ELEVATOR_ARRIVES;

            floor = 1;
        }

        public void GoTo2nd()
        {
            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;
            Info.Floor2.CallEnabled = false;
            Info.Floor2.Screen = Constants.ELEVATOR_ARRIVES;
            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;

            floor = 2;
        }

        public void GoTo3rd()
        {
            Info.Floor3.CallEnabled = false;
            Info.Floor3.Screen = Constants.ELEVATOR_ARRIVES;
            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;
            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;

            floor = 3;
        }

        public void CallTo1st()
        {
            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;
            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;
            Info.Floor1.CallEnabled = false;
            Info.Floor1.Screen = Constants.ELEVATOR_CALLED_DOWN;

            floor = 1;
        }

        public void CallTo2nd()
        {
            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;
            Info.Floor2.CallEnabled = false;
            Info.Floor2.Screen = floor == 1 ? Constants.ELEVATOR_CALLED_UP : Constants.ELEVATOR_CALLED_DOWN;
            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;

            floor = 2;
        }

        public void CallTo3rd()
        {
            Info.Floor3.CallEnabled = false;
            Info.Floor3.Screen = Constants.ELEVATOR_CALLED_UP;
            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;
            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;

            floor = 3;
        }

        //

        private int floor;
    }
}