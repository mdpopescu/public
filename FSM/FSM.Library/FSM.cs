using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Renfield.FSM.Library
{
    public class FSM<T>
    {
        public bool IgnoreErrors { get; set; }
        // ReSharper disable once InconsistentNaming
        public Action OnStart;

        public FSM(string initialState)
        {
            this.initialState = initialState;

            nodes = new List<Node<T>>();
            Restart();
        }

        public void Restart()
        {
            current = GetNode(initialState);
            OnStart?.Invoke();
        }

        public void Handle(T input)
        {
            var next = current.Handle(input);
            if (next != null)
                current = next;
            else if (!IgnoreErrors)
                throw new Exception($"Unknown input [{input}]");
        }

        public void Add(string state, Predicate<T> predicate, Action<T> action, string newState = null)
        {
            var node = GetNode(state);
            node.Add(predicate, action, GetNode(newState ?? state));
        }

        public void Add(string state, T value, Action<T> action, string newState = null)
        {
            var node = GetNode(state);
            node.Add(value, action, GetNode(newState ?? state));
        }

        //

        private readonly string initialState;
        private readonly List<Node<T>> nodes;
        private Node<T> current;

        private Node<T> GetNode(string state)
        {
            var node = nodes.FirstOrDefault(it => string.Compare(it.State, state, true, CultureInfo.InvariantCulture) == 0);
            if (node != null)
                return node;

            node = new Node<T>(state);
            nodes.Add(node);

            return node;
        }
    }
}