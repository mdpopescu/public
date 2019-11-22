namespace TransportTycoon.Library.Models
{
    public class Vehicle
    {
        public Vehicle(VehicleType type, string id, Endpoint startingPoint)
        {
            this.type = type;
            this.id = id;

            location = startingPoint;
        }

        //

        private readonly VehicleType type;
        private readonly string id;

        private Endpoint location;
    }
}