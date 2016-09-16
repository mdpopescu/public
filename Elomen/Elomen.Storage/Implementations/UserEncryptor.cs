using System.Security.Cryptography;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class UserEncryptor : Encoder<string, string>
    {
        public string Encode(string value)
        {
            return ProtectedDataAdapter.Protect(value, null, DataProtectionScope.CurrentUser);
        }

        public string Decode(string value)
        {
            return ProtectedDataAdapter.Unprotect(value, null, DataProtectionScope.CurrentUser);
        }
    }
}