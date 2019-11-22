using System.Collections.Generic;

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
            costs.Add(link, cost);
        }

        public Vehicle Create(string id, Endpoint startingPoint) => new Vehicle(this, id, startingPoint);

        public int GetCost(Endpoint origin, Endpoint destination) => costs[new Link(origin, destination)];

        //

        private readonly Dictionary<Link, int> costs = new Dictionary<Link, int>();
    }
}