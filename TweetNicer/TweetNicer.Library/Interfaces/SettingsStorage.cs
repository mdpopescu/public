namespace TweetNicer.Library.Interfaces
{
    public interface SettingsStorage<in TKey, TValue>
    {
        TValue LoadUserSettings(TKey source);
        TValue LoadMachineSettings(TKey source);

        void SaveUserSettings(TKey destination, TValue settings);
        void SaveMachineSettings(TKey destination, TValue settings);
    }
}