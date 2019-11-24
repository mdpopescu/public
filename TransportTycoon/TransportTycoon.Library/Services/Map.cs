#nullable enable

using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class Map : IMap
    {
        public Map(IEnumerable<Link> links)
        {
            this.links = links.ToArray();
        }

        public IEnumerable<Link> GetRoute(Endpoint origin, Endpoint destination)
        {
            var nodes = GetNodes().ToArray();
            var solution = Dijkstra(nodes, origin, destination);
            var path = BuildPath(solution, origin, destination).ToArray();
            return FindLinks(path);
        }

        //

        private readonly Link[] links;

        private IEnumerable<Endpoint> GetNodes()
        {
            var result = new HashSet<Endpoint>();

            foreach (var link in links)
            {
                result.Add(link.E1);
                result.Add(link.E2);
            }

            return result;
        }

        private IReadOnlyDictionary<Endpoint, Endpoint?> Dijkstra(IEnumerable<Endpoint> nodes, Endpoint origin, Endpoint destination)
        {
            // Dijkstra's shortest path algorithm -- based on https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

            var unvisited = new List<Endpoint>();
            var dist = new Dictionary<Endpoint, int>();
            var prev = new Dictionary<Endpoint, Endpoint?>();

            Initialize();

            while (unvisited.Any())
            {
                var nearest = unvisited.OrderBy(it => dist[it]).First();
                unvisited.Remove(nearest);

                if (nearest == destination)
                    return prev;

                UpdateCosts(nearest);
            }

            return new Dictionary<Endpoint, Endpoint?>();

            //

            void Initialize()
            {
                foreach (var v in nodes)
                {
                    dist[v] = int.MaxValue;
                    prev[v] = null;
                    unvisited.Add(v);
                }

                dist[origin] = 0;
            }

            void UpdateCosts(Endpoint e)
            {
                // all links containing e where the other endpoint is unvisited
                var unvisitedLinks = links.Where(it => it.Contains(e) && unvisited.Contains(it.GetOther(e)));
                foreach (var link in unvisitedLinks)
                    UpdateCost(link, e);
            }

            void UpdateCost(Link link, Endpoint e)
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