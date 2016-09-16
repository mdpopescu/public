namespace Elomen.Storage.Contracts
{
    public interface ResourceStore<T>
    {
        T Load();
        void Save(T value);
    }
}