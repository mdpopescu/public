namespace EverythingIsADatabase.Logic.Models
{
    public class Attribute
    {
        public string Name { get; }
        public object Value { get; set; }

        public Attribute(string name, object value = null)
        {
            Name = name;
            Value = value;
        }

        public bool Matches(Attribute other)
        {
            // if the names don't match, return false
            // if the other has no value, return true
            // if the other has value and this one doesn't, return false
            // if the other has value and it equals this one's, return true
            // otherwise return false

            return other.Name == Name &&
                   (other.Value == null || (Value != null && other.Value.Equals(Value)));
        }
    }
}