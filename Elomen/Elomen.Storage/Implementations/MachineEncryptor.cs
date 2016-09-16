using System.Security.Cryptography;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class MachineEncryptor : Encoder<string, string>
    {
        public MachineEncryptor(string password)
        {
            this.password = password;
        }

        public string Encode(string value)
        {
            return ProtectedDataAdapter.Protect(value, password, DataProtectionScope.LocalMachine);
        }

        public string Decode(string value)
        {
            return ProtectedDataAdapter.Unprotect(value, password, DataProtectionScope.LocalMachine);
        }

        //

        private readonly string password;
    }
}