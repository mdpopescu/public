namespace TweetNicer.Library.Interfaces
{
    public interface SecureStorage
    {
        string LoadUserData(string path);
        string LoadMachineData(string path, string password);

        void SaveUserData(string path, string data);
        void SaveMachineData(string path, string password, string data);
    }
}