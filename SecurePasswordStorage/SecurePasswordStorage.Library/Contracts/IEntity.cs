using System;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface IEntity<TKey> : IEquatable<Entity<TKey>>
    {
        TKey Key { get; }
    }
}