namespace TweetNicer.Library.Interfaces
{
    public interface Storage<in TKey, TValue>
    {
        TValue LoadUserValues(TKey key);
        TValue LoadMachineValues(TKey key);

        void SaveUserValues(TKey key, TValue settings);
        void SaveMachineValues(TKey key, TValue settings);
    }
}