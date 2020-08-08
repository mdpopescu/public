using System.Collections.Generic;
using System.Linq;
using System.Text;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Helpers
{
    public static class Extensions
    {
        public static byte[] GetSaltedCredentials(this Credentials credentials, IEnumerable<byte> salt) =>
            salt
                .Concat(Encoding.UTF8.GetBytes(credentials.Key.Value))
                .Concat(Encoding.UTF8.GetBytes(credentials.Password))
                .ToArray();
    }
}