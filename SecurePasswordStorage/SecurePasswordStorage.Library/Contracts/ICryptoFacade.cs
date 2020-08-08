using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        byte[] GenerateSalt();

        void Transform(Credentials credentials, out byte[] secretKey, out byte[] verificationHash);

        bool VerifyHash(byte[] expected, byte[] actual);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}