using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsSecureStorage : ValueStore<string>
    {
        public WindowsSecureStorage(ResourceStore<string> store, Encoder<string, string> userEncryptor, Encoder<string, string> machineEncryptor)
        {
            this.store = store;
            this.userEncryptor = userEncryptor;
            this.machineEncryptor = machineEncryptor;
        }

        public string UserValues
        {
            get { return userEncryptor.Decode(store.Load()); }
            set { store.Save(userEncryptor.Decode(value)); }
        }

        public string MachineValues
        {
            get { return machineEncryptor.Decode(store.Load()); }
            set { store.Save(machineEncryptor.Decode(value)); }
        }

        //

        private readonly ResourceStore<string> store;
        private readonly Encoder<string, string> userEncryptor;
        private readonly Encoder<string, string> machineEncryptor;
    }
}