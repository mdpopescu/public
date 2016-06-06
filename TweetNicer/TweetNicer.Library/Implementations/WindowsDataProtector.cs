using System;
using System.Security.Cryptography;
using System.Text;
using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class WindowsDataProtector : DataProtectorAdapter
    {
        public string EncryptForUser(string data)
        {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var encrypted = ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public string EncryptForMachine(string data, string password)
        {
            var plaintext = Encoding.UTF8.GetBytes(data);
            var secret = Encoding.UTF8.GetBytes(password);
            var encrypted = ProtectedData.Protect(plaintext, secret, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptForUser(string data)
        {
            var encrypted = Convert.FromBase64String(data);
            var plaintext = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plaintext);
        }

        public string DecryptForMachine(string data, string password)
        {
            var encrypted = Convert.FromBase64String(data);
            var secret = Encoding.UTF8.GetBytes(password);
            var plaintext = ProtectedData.Unprotect(encrypted, secret, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plaintext);
        }
    }
}