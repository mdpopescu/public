#nullable enable

using System;

namespace TransportTycoon.Library.Models
{
    public class Link : IEquatable<Link?>
    {
        public Endpoint E1 { get; }
        public Endpoint E2 { get; }
        public int Cost { get; }

        public Link(Endpoint e1, Endpoint e2, int cost)
        {
            E1 = e1;
            E2 = e2;
            Cost = cost;
        }

        #region equality

        public bool Equals(Link? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            // ReSharper disable ArrangeRedundantParentheses
            return (Equals(E1, other.E1) && Equals(E2, other.E2))
                || (Equals(E1, other.E2) && Equals(E2, other.E1));
            // ReSharper restore ArrangeRedundantParentheses
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Link) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (E1.GetHashCode() * 397) ^ E2.GetHashCode();
            }
        }

        public static bool operator ==(Link? left, Link? right) => ReferenceEquals(left, right) || !(left is null) && left.Equals(right);
        public static bool operator !=(Link? left, Link? right) => !(left == right);

        #endregion

        public bool Contains(Endpoint e) => E1 == e || E2 == e;

        public Endpoint GetOther(Endpoint e) => e == E1 ? E2 : E1;
    }
}