using System;

namespace Renfield.FSM.Library
{
    public class Edge<T>
    {
        public Predicate<T> Predicate { get; }
        public Action<T> Action { get; }
        public Node<T> Node { get; }

        public Edge(Predicate<T> predicate, Action<T> action, Node<T> node)
        {
            Predicate = predicate;
            Action = action;
            Node = node;
        }
    }
}