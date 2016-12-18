using System;
using System.Security.Cryptography;
using CustomCrypto.Library.Contracts;

namespace CustomCrypto.Library.Implementations
{
    public class SecureRNG : RNG, IDisposable
    {
        public SecureRNG()
        {
            rng = new RNGCryptoServiceProvider();
        }

        public void Dispose()
        {
            rng.Dispose();
        }

        public uint Next()
        {
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        //

        private readonly RNGCryptoServiceProvider rng;
    }
}