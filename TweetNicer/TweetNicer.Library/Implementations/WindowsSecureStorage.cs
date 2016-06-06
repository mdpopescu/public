using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class WindowsSecureStorage : SecureStorage
    {
        public WindowsSecureStorage(FileSystem fs, DataProtectorAdapter protector)
        {
            this.fs = fs;
            this.protector = protector;
        }

        public string LoadUserData(string path)
        {
            return protector.DecryptForUser(fs.Load(path));
        }

        public string LoadMachineData(string path, string password)
        {
            return protector.DecryptForMachine(fs.Load(path), password);
        }

        public void SaveUserData(string path, string data)
        {
            fs.Save(path, protector.EncryptForUser(data));
        }

        public void SaveMachineData(string path, string password, string data)
        {
            fs.Save(path, protector.EncryptForMachine(data, password));
        }

        //

        private readonly FileSystem fs;
        private readonly DataProtectorAdapter protector;
    }
}