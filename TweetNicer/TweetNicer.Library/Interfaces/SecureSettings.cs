namespace TweetNicer.Library.Interfaces
{
    public interface SecureSettings
    {
        Settings LoadUserSettings(string path);
        Settings LoadMachineSettings(string path, string password);

        void SaveUserSettings(string path, Settings settings);
        void SaveMachineSettings(string path, string password, Settings settings);
    }
}