namespace TweetNicer.Library.Interfaces
{
    public interface Storage<in TKey, TValue>
    {
        TValue LoadUserValues(TKey source);
        TValue LoadMachineValues(TKey source);

        void SaveUserValues(TKey destination, TValue settings);
        void SaveMachineValues(TKey destination, TValue settings);
    }
}