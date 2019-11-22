using System;

namespace TransportTycoon.Library.Models
{
    public class Endpoint : IEquatable<Endpoint>
    {
        public string Name { get; }

        public Endpoint(string name)
        {
            Name = name;
        }

        public bool Equals(Endpoint other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Endpoint) obj);
        }

        public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;

        public static bool operator ==(Endpoint left, Endpoint right) => Equals(left, right);

        public static bool operator !=(Endpoint left, Endpoint right) => !Equals(left, right);
    }
}