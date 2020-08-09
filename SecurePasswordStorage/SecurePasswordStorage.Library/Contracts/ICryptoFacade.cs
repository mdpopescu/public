using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        ByteArrayTuple GenerateHash(Credentials credentials);

        ByteArrayTuple Transform(Credentials credentials);
        bool VerifyHash(Credentials credentials, ByteArrayTuple saltedHash);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}