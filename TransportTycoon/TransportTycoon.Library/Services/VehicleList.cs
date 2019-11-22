using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class VehicleList : IVehicleList
    {
        public VehicleList(IEnumerable<Vehicle> vehicles)
        {
            this.vehicles = vehicles.ToArray();
        }

        public Vehicle FindAvailable(Endpoint location)
        {
            return vehicles.First(it => it.Location == location);
        }

        //

        private readonly Vehicle[] vehicles;
    }
}