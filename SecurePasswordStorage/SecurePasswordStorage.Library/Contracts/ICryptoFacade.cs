using System;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        void TransformPassword(string password, out byte[] secretKey, out byte[] verificationHash);

        Tuple<byte[], byte[]> LargeHash(object value);
        byte[] SecureHash(object value);

        byte[] Encrypt(byte[] key, byte[] decrypted);
        byte[] Decrypt(byte[] key, byte[] encrypted);
    }
}