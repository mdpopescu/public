namespace SecurePasswordStorage.Library.Contracts
{
    public interface IRepository<T, in TKey>
        where T : IEntity<TKey>
    {
        /// <summary>
        ///     Returns the entity with the given key, or <c>null</c> if one was not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The entity with the given key or <c>null</c> if one was not found.</returns>
        T Load(TKey key);

        /// <summary>
        ///     Adds or updates the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Save(T entity);
    }
}