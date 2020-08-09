using System;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        byte[] GenerateSalt();

        byte[] GetBytes(Credentials credentials);
        byte[] SecureHash(byte[] value, byte[] salt);

        Tuple<byte[],byte[]> GenerateHash(Credentials credentials);

        void Transform(Credentials credentials, out byte[] secretKey, out byte[] verificationHash);
        bool VerifyHash(Credentials credentials, byte[] salt, byte[] passwordHash);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}