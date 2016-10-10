using System;
using System.Collections.Generic;
using System.Linq;

namespace Renfield.FSM.Library
{
    public class Node<T>
    {
        public string State { get; private set; }

        public Node(string state)
        {
            State = state;
            edges = new List<Edge<T>>();
        }

        public void Add(Predicate<T> predicate, Action<T> action, Node<T> node = null)
        {
            edges.Add(new Edge<T>(predicate, action, node ?? this));
        }

        public void Add(T value, Action<T> action, Node<T> node = null)
        {
            Add(it => it.Equals(value), action, node);
        }

        public Node<T> Handle(T input)
        {
            var edge = edges.FirstOrDefault(e => e.Predicate != null && e.Predicate(input)) ??
                       edges.FirstOrDefault(e => e.Predicate == null);
            if (edge == null)
                return null;

            edge.Action?.Invoke(input);

            return edge.Node;
        }

        //

        private readonly List<Edge<T>> edges;
    }
}