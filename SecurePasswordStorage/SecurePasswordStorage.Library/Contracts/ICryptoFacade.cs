using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        void Transform(Credentials credentials, out byte[] secretKey, out byte[] verificationHash);

        byte[] SecureHash(byte[] value);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}