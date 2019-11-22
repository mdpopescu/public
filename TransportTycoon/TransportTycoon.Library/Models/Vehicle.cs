namespace TransportTycoon.Library.Models
{
    public class Vehicle
    {
        public Endpoint Location { get; private set; }

        public Vehicle(VehicleType type, string id, Endpoint home)
        {
            this.type = type;
            this.id = id;
            this.home = home;

            Location = home;
        }

        public int GetCost(Endpoint destination)
        {
            // assume that the vehicle is at an endpoint (instead of along the way) and there is a direct link to the destination
            return type.GetCost(Location, destination);
        }

        public void MoveTo(Endpoint destination)
        {
            Location = destination;
        }

        //

        private readonly VehicleType type;
        private readonly string id;
        private readonly Endpoint home;
    }
}