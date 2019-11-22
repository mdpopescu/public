using System;

namespace TransportTycoon.Library.Models
{
    public class Link : IEquatable<Link>
    {
        public Endpoint E1 { get; }
        public Endpoint E2 { get; }

        public Link(Endpoint e1, Endpoint e2)
        {
            E1 = e1;
            E2 = e2;
        }

        public bool Equals(Link other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(E1, other.E1)
                && Equals(E2, other.E2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Link) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((E1 != null ? E1.GetHashCode() : 0) * 397) ^ (E2 != null ? E2.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Link left, Link right) => Equals(left, right);

        public static bool operator !=(Link left, Link right) => !Equals(left, right);
    }
}