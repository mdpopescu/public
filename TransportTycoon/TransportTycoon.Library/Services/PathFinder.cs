#nullable enable

using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class PathFinder
    {
        public PathFinder(IEnumerable<Endpoint> nodes, IEnumerable<Link> links)
        {
            this.nodes = nodes.ToArray();
            this.links = links.ToArray();
        }

        public IReadOnlyDictionary<Endpoint, Endpoint?> FindShortestPath(Endpoint origin, Endpoint destination)
        {
            // Dijkstra's shortest path algorithm -- based on https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

            Initialize(origin);

            while (unvisited.Any())
            {
                var nearest = unvisited.OrderBy(it => dist[it]).First();
                unvisited.Remove(nearest);

                if (nearest == destination)
                    return prev;

                UpdateCosts(nearest);
            }

            return new Dictionary<Endpoint, Endpoint?>();
        }

        //

        private readonly List<Endpoint> unvisited = new List<Endpoint>();
        private readonly Dictionary<Endpoint, int> dist = new Dictionary<Endpoint, int>();
        private readonly Dictionary<Endpoint, Endpoint?> prev = new Dictionary<Endpoint, Endpoint?>();

        private readonly Endpoint[] nodes;
        private readonly Link[] links;

        //

        private void Initialize(Endpoint origin)
        {
            unvisited.Clear();
            dist.Clear();
            prev.Clear();

            foreach (var v in nodes)
            {
                dist[v] = int.MaxValue;
                prev[v] = null;
                unvisited.Add(v);
            }

            dist[origin] = 0;
        }

        private void UpdateCosts(Endpoint e)
        {
            // all links containing e where the other endpoint is unvisited
            var unvisitedLinks = links.Where(it => it.Contains(e) && unvisited.Contains(it.GetOther(e)));
            foreach (var link in unvisitedLinks)
                UpdateCost(link, e);
        }

        private void UpdateCost(Link link, Endpoint e)
        {
            var v = link.GetOther(e);

            var alt = dist[e] + link.Cost;
            // ReSharper disable once InvertIf
            if (alt < dist[v])
            {
                dist[v] = alt;
                prev[v] = e;
            }
        }
    }
}