namespace Elomen.Library.Contracts
{
    public interface Repository<in TKey, out TValue>
    {
        TValue Find(TKey id);
    }
}