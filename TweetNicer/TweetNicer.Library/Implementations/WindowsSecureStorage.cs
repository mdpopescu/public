using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class WindowsSecureStorage : SettingsStorage<string, string>
    {
        public WindowsSecureStorage(FileSystem fs, DataProtectorAdapter protector, string password)
        {
            this.fs = fs;
            this.protector = protector;
            this.password = password;
        }

        public string LoadUserSettings(string path)
        {
            return protector.DecryptForUser(fs.Load(path));
        }

        public string LoadMachineSettings(string path)
        {
            return protector.DecryptForMachine(fs.Load(path), password);
        }

        public void SaveUserSettings(string path, string data)
        {
            fs.Save(path, protector.EncryptForUser(data));
        }

        public void SaveMachineSettings(string path, string settings)
        {
            fs.Save(path, protector.EncryptForMachine(settings, password));
        }

        //

        private readonly FileSystem fs;
        private readonly DataProtectorAdapter protector;
        private readonly string password;
    }
}