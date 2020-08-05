using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICrypto
    {
        SecureHash GetSecureHash(object value);
        LargeHash GetLargeHash(object value);

        byte[] Encrypt(Key key, object value);
    }
}