namespace TransportTycoon.Library.Models
{
    public class VehicleType
    {
        public string Type { get; }

        public VehicleType(string type)
        {
            Type = type;
        }

        public void SetCost(Link link, int cost)
        {
            //
        }

        public Vehicle Create(string id, Endpoint startingPoint)
        {
            return new Vehicle(this, id, startingPoint);
        }
    }
}