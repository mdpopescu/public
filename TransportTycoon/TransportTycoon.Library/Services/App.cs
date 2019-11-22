using System.Collections.Generic;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map, IVehicleList vehicles)
        {
            this.map = map;
            this.vehicles = vehicles;
        }

        public int Run(Endpoint origin, IEnumerable<Endpoint> destinations)
        {
            var total = 0;
            foreach (var destination in destinations)
            {
                // assume that a vehicle is available at the origin
                var vehicle = vehicles.FindAvailable(origin);

                // assume that a direct link exists from origin to destination
                total += vehicle.GetCost(destination);

                vehicle.MoveTo(destination);
            }

            return total;
        }

        //

        private readonly IMap map;
        private readonly IVehicleList vehicles;
    }
}