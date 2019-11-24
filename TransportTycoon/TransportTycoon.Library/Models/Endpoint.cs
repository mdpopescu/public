#nullable enable

using System;

namespace TransportTycoon.Library.Models
{
    public class Endpoint : IEquatable<Endpoint?>
    {
        public string Name { get; }

        public Endpoint(string name)
        {
            Name = name;
        }

        #region equality

        public bool Equals(Endpoint? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Endpoint) obj);
        }

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(Endpoint? left, Endpoint? right) => ReferenceEquals(left, right) || !(left is null) && left.Equals(right);
        public static bool operator !=(Endpoint? left, Endpoint? right) => !(left == right);

        #endregion
    }
}