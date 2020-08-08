using System;

namespace SecurePasswordStorage.Library.Contracts
{
    public interface ICryptoFacade
    {
        Tuple<byte[], byte[]> LargeHash(object value);
        byte[] SecureHash(object value);

        byte[] Encrypt(byte[] key, byte[] value);
    }
}