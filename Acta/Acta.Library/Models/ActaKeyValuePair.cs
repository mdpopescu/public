namespace Acta.Library.Models
{
    public class ActaKeyValuePair
    {
        public string Name { get; }
        public object Value { get; }

        public ActaKeyValuePair(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}