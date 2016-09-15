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
            get { return encryptor.DecryptForUser(fs.Load(path)); }
            set { fs.Save(path, encryptor.EncryptForUser(value)); }
        }

        public string MachineValues
        {
            get { return encryptor.DecryptForMachine(fs.Load(path)); }
            set { fs.Save(path, encryptor.EncryptForMachine(value)); }
        }

        //

        private readonly string path;
        private readonly FileSystem fs;
        private readonly DataEncryptor encryptor;
    }
}