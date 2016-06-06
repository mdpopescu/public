namespace TweetNicer.Library.Interfaces
{
    public interface SecureSettings<in T>
    {
        Settings LoadUserSettings(T source);
        Settings LoadMachineSettings(T source, string password);

        void SaveUserSettings(T destination, Settings settings);
        void SaveMachineSettings(T destination, string password, Settings settings);
    }
}