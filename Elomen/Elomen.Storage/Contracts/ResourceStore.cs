namespace Elomen.Storage.Contracts
{
    /// <summary>
    /// Generic resource store.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    public interface ResourceStore<T> : Loadable<T>, Saveable<T>
    {
    }
}