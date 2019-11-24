#nullable enable

using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class Map : IMap
    {
        public Map(IEnumerable<Endpoint> nodes, IEnumerable<Link> links)
        {
            this.nodes = nodes.ToArray();
            this.links = links.ToArray();
        }

        public IEnumerable<Link> GetRoute(Endpoint origin, Endpoint destination)
        {
            var pathFinder = new PathFinder(nodes, links);
            var solution = pathFinder.FindShortestPath(origin, destination);

            //var solution = Dijkstra(nodes, origin, destination);
            var path = BuildPath(solution, origin, destination).ToArray();
            return FindLinks(path);
        }

        //

        private readonly Endpoint[] nodes;
        private readonly Link[] links;

        private static IEnumerable<Endpoint> BuildPath(IReadOnlyDictionary<Endpoint, Endpoint?> prev, Endpoint origin, Endpoint destination)
        {
            var u = destination;
            if (u != origin && prev[u] == null)
                return Enumerable.Empty<Endpoint>();

            var result = new List<Endpoint>();

            // ReSharper disable once SuggestVarOrType_SimpleTypes
            Endpoint? uu = u;
            while (uu != null)
            {
                result.Add(uu);
                uu = prev[uu];
            }

            return result;
        }

        private IEnumerable<Link> FindLinks(IReadOnlyList<Endpoint> endpoints)
        {
            var origin = endpoints.First();
            foreach (var destination in endpoints.Skip(1))
            {
                yield return links.First(it => it == new Link(origin, destination, 0));
                origin = destination;
            }
        }
    }
}