using System;

namespace Acta.Library.Models
{
    public class ActaTuple
    {
        public Guid Id { get; }
        public string Name { get; }
        public object Value { get; }
        public long Timestamp { get; }

        public ActaTuple(Guid id, string name, object value, long timestamp)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name cannot be null or whitespace.");

            Id = id;
            Name = name.ToUpperInvariant();
            Value = value;
            Timestamp = timestamp;
        }

        public bool Matches(Guid guid, string name)
        {
            return GuidsMatch(guid) && NamesMatch(name);
        }

        public bool Matches(string name, object value)
        {
            return NamesMatch(name) && ValuesMatch(value);
        }

        public bool NamesMatch(string name)
        {
            return string.Compare(Name, name, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        //

        private bool GuidsMatch(Guid guid)
        {
            return Id == guid;
        }

        private bool ValuesMatch(object value)
        {
            return Value == value || Value != null && value != null && Value.Equals(value);
        }
    }
}