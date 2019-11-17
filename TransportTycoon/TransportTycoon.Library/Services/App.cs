using System;
using System.Linq;
using TransportTycoon.Library.Contracts;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map)
        {
            this.map = map;
        }

        public int Run(string containers)
        {
            if (string.IsNullOrEmpty(containers))
                throw new ArgumentException("At least one destination is required.", nameof(containers));

            var routes = map.Routes.ToArray();
            if (!routes.Any())
                throw new InvalidOperationException("At least one route needs to be defined.");

            return 0;
        }

        //

        private readonly IMap map;
    }
}