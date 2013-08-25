using System;

namespace Renfield.FSM.Library
{
  public class FSM<T>
  {
    public bool IgnoreErrors { get; set; }

    public FSM(Node<T> start)
    {
      current = start;
    }

    public void Handle(T input)
    {
      var next = current.Handle(input);
      if (next != null)
        current = next;
      else if (!IgnoreErrors)
        throw new Exception(string.Format("Unknown input [{0}]", input));
    }

    //

    private Node<T> current;
  }
}