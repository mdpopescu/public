using System;

namespace EverythingIsADatabase.Logic.Models
{
    public class AttributeMatch
    {
        public string Name { get; }
        public Func<object, bool> Predicate { get; }

        public AttributeMatch(string name, Func<object, bool> predicate)
        {
            Name = name;
            Predicate = predicate;
        }
    }
}