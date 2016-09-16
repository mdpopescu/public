using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsSecureStorage : GenericStorage<string>
    {
        public WindowsSecureStorage(ResourceStore<string> store, DataEncryptor encryptor)
        {
            this.store = store;
            this.encryptor = encryptor;
        }

        public string UserValues
        {
            get { return encryptor.DecryptForUser(store.Load()); }
            set { store.Save(encryptor.EncryptForUser(value)); }
        }

        public string MachineValues
        {
            get { return encryptor.DecryptForMachine(store.Load()); }
            set { store.Save(encryptor.EncryptForMachine(value)); }
        }

        //

        private readonly ResourceStore<string> store;
        private readonly DataEncryptor encryptor;
    }
}