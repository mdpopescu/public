namespace Elomen.Storage.Contracts
{
    /// <summary>
    /// Generic resource store.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    public interface ResourceStore<T>
    {
        T Load();
        void Save(T value);
    }
}