using System;
using System.Security.Cryptography;
using System.Text;

namespace Elomen.Storage.Implementations
{
    public static class ProtectedDataAdapter
    {
        public static string Protect(string data, string password, DataProtectionScope scope)
        {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = ProtectedData.Protect(plaintext, GetSecret(password), scope);
            return Convert.ToBase64String(encrypted);
        }

        public static string Unprotect(string data, string password, DataProtectionScope scope)
        {
            var encrypted = Convert.FromBase64String(data);
            var plaintext = ProtectedData.Unprotect(encrypted, GetSecret(password), scope);
            return Encoding.UTF8.GetString(plaintext);
        }

        //

        private static byte[] GetSecret(string password)
        {
            return password == null ? null : Encoding.UTF8.GetBytes(password);
        }
    }
}