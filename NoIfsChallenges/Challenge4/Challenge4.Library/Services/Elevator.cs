using Challenge4.Library.Models;

namespace Challenge4.Library.Services
{
    public class Elevator
    {
        public ElevatorInfo Initialize()
        {
            var result = new ElevatorInfo();

            result.Floor3.Button1Enabled = true;
            result.Floor3.Button2Enabled = false;
            result.Floor3.Button3Enabled = false;
            result.Floor3.Screen = "Door Closed";

            result.Floor2.Button1Enabled = true;
            result.Floor2.Button2Enabled = false;
            result.Floor2.Button3Enabled = false;
            result.Floor2.Screen = "Door Closed";

            result.Floor1.Button1Enabled = false;
            result.Floor1.Button2Enabled = true;
            result.Floor1.Button3Enabled = true;
            result.Floor1.Screen = "Door Open";

            return result;
        }
    }
}