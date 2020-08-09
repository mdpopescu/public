using System.Linq;
using System.Text;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Helpers
{
    public static class Extensions
    {
        public static byte[] GetBytes(this Credentials credentials) =>
            Encoding.UTF8.GetBytes(credentials.Key.Value)
                .Concat(Encoding.UTF8.GetBytes(credentials.Password))
                .ToArray();
    }
}