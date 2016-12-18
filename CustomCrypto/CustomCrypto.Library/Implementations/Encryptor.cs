using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomCrypto.Library.Contracts;

namespace CustomCrypto.Library.Implementations
{
    public class Encryptor
    {
        public Encryptor(string key)
        {
            keyRNG = new PredictableRNG(key.GetHashCode());
            outputRNG = new SecureRNG();
        }

        public uint[] Encrypt(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var bits = new BitArray(bytes);

            return bits
                .Cast<bool>()
                .Select(EncryptBit)
                .ToArray();
        }

        public string Decrypt(uint[] encrypted)
        {
            var bits = new BitArray(encrypted.Select(DecryptBit).ToArray());
            var bytes = BitArrayToByteArray(bits).ToArray();

            return Encoding.UTF8.GetString(bytes);
        }

        //

        private readonly RNG keyRNG;
        private readonly RNG outputRNG;

        private uint EncryptBit(bool bit)
        {
            var output = outputRNG.Next();

            var index = (int) (keyRNG.Next() % 32);
            var mask = (uint) 1 << index;

            output &= ~mask;
            if (bit)
                output |= mask;

            return output;
        }

        private bool DecryptBit(uint encrypted)
        {
            var index = (int) (keyRNG.Next() % 32);
            var mask = (uint) 1 << index;

            return (encrypted & mask) != 0;
        }

        private static IEnumerable<byte> BitArrayToByteArray(BitArray bits)
        {
            var ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);

            return ret;
        }
    }
}