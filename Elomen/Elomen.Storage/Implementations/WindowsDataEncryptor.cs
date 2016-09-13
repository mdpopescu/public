using System.Security.Cryptography;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsDataEncryptor : DataEncryptor
    {
        public string Encrypt(Location location, string data, string password)
        {
            return location == Location.User
                ? ProtectedDataAdapter.Protect(data, null, DataProtectionScope.CurrentUser)
                : ProtectedDataAdapter.Protect(data, password, DataProtectionScope.LocalMachine);
        }

        public string Decrypt(Location location, string data, string password)
        {
            return location == Location.User
                ? ProtectedDataAdapter.Unprotect(data, null, DataProtectionScope.CurrentUser)
                : ProtectedDataAdapter.Unprotect(data, password, DataProtectionScope.LocalMachine);
        }
    }
}