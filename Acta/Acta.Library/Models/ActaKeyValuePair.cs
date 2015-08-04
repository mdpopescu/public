namespace Acta.Library.Models
{
  public class ActaKeyValuePair
  {
    public string Name { get; private set; }
    public object Value { get; private set; }

    public ActaKeyValuePair(string name, object value)
    {
      Name = name;
      Value = value;
    }
  }
}