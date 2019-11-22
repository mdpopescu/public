using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class Vehicle : IVehicle
    {
        public string Type { get; }

        public Vehicle(string type)
        {
            Type = type;
        }

        public void AddRoute(Endpoint e1, Endpoint e2, int cost)
        {
            throw new System.NotImplementedException();
        }
    }
}