using System.Security.Cryptography;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsDataEncryptor : DataEncryptor
    {
        public WindowsDataEncryptor(string password)
        {
            this.password = password;
        }

        public string EncryptForUser(string data)
        {
            return ProtectedDataAdapter.Protect(data, null, DataProtectionScope.CurrentUser);
        }

        public string DecryptForUser(string data)
        {
            return ProtectedDataAdapter.Unprotect(data, null, DataProtectionScope.CurrentUser);
        }

        public string EncryptForMachine(string data)
        {
            return ProtectedDataAdapter.Protect(data, password, DataProtectionScope.LocalMachine);
        }

        public string DecryptForMachine(string data)
        {
            return ProtectedDataAdapter.Unprotect(data, password, DataProtectionScope.LocalMachine);
        }

        //

        private readonly string password;
    }
}