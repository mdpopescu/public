namespace Elomen.Storage.Contracts
{
    public interface Saveable<in T>
    {
        void Save(T value);
    }
}