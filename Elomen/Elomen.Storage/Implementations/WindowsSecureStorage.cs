using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsSecureStorage : GenericStorage<string>
    {
        public WindowsSecureStorage(string path, FileSystem fs, DataEncryptor encryptor)
        {
            this.path = path;
            this.fs = fs;
            this.encryptor = encryptor;
        }

        public string UserValues
        {
            get { return Load(Location.User); }
            set { Save(Location.User, value); }
        }

        public string MachineValues
        {
            get { return Load(Location.Machine); }
            set { Save(Location.Machine, value); }
        }

        //

        private readonly string path;
        private readonly FileSystem fs;
        private readonly DataEncryptor encryptor;

        private string Load(Location location)
        {
            return encryptor.Decrypt(location, fs.Load(path));
        }

        private void Save(Location location, string value)
        {
            fs.Save(path, encryptor.Encrypt(location, value));
        }
    }
}