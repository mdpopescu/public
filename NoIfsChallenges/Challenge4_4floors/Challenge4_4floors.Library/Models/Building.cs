namespace Challenge4_4floors.Library.Models
{
    public class Building
    {
        public Building()
        {
            floors = new[]
            {
                new Floor(true, Constants.DOOR_CLOSED),
                new Floor(false, Constants.DOOR_CLOSED),
                new Floor(false, Constants.DOOR_CLOSED),
                new Floor(false, Constants.DOOR_CLOSED),
            };
        }

        public Floor GetFloor(int index) => floors[index - 1];

        //

        private readonly Floor[] floors;
    }
}