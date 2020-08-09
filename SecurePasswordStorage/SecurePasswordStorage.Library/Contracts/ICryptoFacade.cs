using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        ByteArrayTuple GenerateHash(Credentials credentials);

        void Transform(Credentials credentials, out byte[] secretKey, out byte[] verificationHash);
        bool VerifyHash(Credentials credentials, byte[] salt, byte[] passwordHash);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}