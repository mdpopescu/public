using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SecurePasswordStorage.Library.Contracts;

namespace SecurePasswordStorage.Library.Models
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Key { get; }

        #region IEquatable

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public bool Equals(Entity<TKey> other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key);
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Entity<TKey>) obj);
        }

        public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Key);

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !Equals(left, right);

        #endregion

        //

        protected Entity(TKey key)
        {
            Key = key;
        }
    }
}