using System;

namespace Renfield.FSM.Library
{
  public class Edge<T>
  {
    public Predicate<T> Predicate { get; private set; }
    public Action<T> Action { get; private set; }
    public Node<T> Node { get; private set; }

    public Edge(Predicate<T> predicate, Action<T> action, Node<T> node)
    {
      Predicate = predicate;
      Action = action;
      Node = node;
    }
  }
}