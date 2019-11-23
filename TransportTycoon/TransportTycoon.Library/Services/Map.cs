using System;
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
            // Dijkstra's shortest path algorithm -- based on https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm

            var nodes = GetNodes().ToArray();

            var q = new List<Endpoint>();
            var dist = new Dictionary<Endpoint, int>();
            var prev = new Dictionary<Endpoint, Endpoint>();

            foreach (var v in nodes)
            {
                dist[v] = int.MaxValue;
                prev[v] = null;
                q.Add(v);
            }

            dist[origin] = 0;

            while (q.Any())
            {
                var u = q.OrderBy(it => dist[it]).First();
                q.Remove(u);

                if (u == destination)
                    return FindLinks(BuildPath(prev, origin, destination).Reverse().ToArray());

                foreach (var link in GetLinks(u, q))
                {
                    var v = link.GetOther(u);

                    var alt = dist[u] + link.Cost;
                    if (alt >= dist[v])
                        continue;

                    dist[v] = alt;
                    prev[v] = u;
                }
            }

            return Enumerable.Empty<Link>();
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

        private static IEnumerable<Endpoint> BuildPath(IReadOnlyDictionary<Endpoint, Endpoint> prev, Endpoint origin, Endpoint destination)
        {
            var u = destination;
            if (u != origin && prev[u] == null)
                yield break;

            while (u != null)
            {
                yield return u;
                u = prev[u];
            }
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

        // return all links containing u where the other endpoint is in q
        private IEnumerable<Link> GetLinks(Endpoint u, IReadOnlyList<Endpoint> q)
        {
            return links.Where(it => it.Contains(u) && q.Contains(it.GetOther(u))).ToArray();
        }
    }
}