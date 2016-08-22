using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class WindowsSecureStorage : Storage<string, string>
    {
        public WindowsSecureStorage(FileSystem fs, DataProtectorAdapter protector, string password)
        {
            this.fs = fs;
            this.protector = protector;
            this.password = password;
        }

        public string LoadUserValues(string path)
        {
            return protector.DecryptForUser(fs.Load(path));
        }

        public string LoadMachineValues(string path)
        {
            return protector.DecryptForMachine(fs.Load(path), password);
        }

        public void SaveUserValues(string path, string data)
        {
            fs.Save(path, protector.EncryptForUser(data));
        }

        public void SaveMachineValues(string path, string settings)
        {
            fs.Save(path, protector.EncryptForMachine(settings, password));
        }

        //

        private readonly FileSystem fs;
        private readonly DataProtectorAdapter protector;
        private readonly string password;
    }
}