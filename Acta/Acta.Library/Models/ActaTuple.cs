using System;

namespace Acta.Library.Models
{
  public class ActaTuple
  {
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public object Value { get; private set; }

    public ActaTuple(Guid id, string name, object value)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("The name cannot be null or whitespace.");

      Id = id;
      Name = name;
      Value = value;
    }
  }
}