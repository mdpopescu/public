using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Helpers;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class CryptoFacade : ICryptoFacade
    {
        public ByteArrayTuple GenerateHash(Credentials credentials)
        {
            var salt = GenerateSalt();
            var hash = SecureHash(credentials.GetBytes(), salt);
            return ByteArrayTuple.Create(salt, hash);
        }

        public ByteArrayTuple Transform(Credentials credentials)
        {
            var bytes = credentials.GetBytes();
            var largeHash = LargeHash(bytes);
            var hash1 = largeHash.Take(largeHash.Length / 2).ToArray();
            var hash2 = largeHash.Skip(largeHash.Length / 2).ToArray();

            var salt = LargeHash(largeHash);
            var secretKey = SecureHash(hash1, salt);
            var verificationHash = SecureHash(hash2, salt);
            return ByteArrayTuple.Create(secretKey, verificationHash);
        }

        public bool VerifyHash(Credentials credentials, ByteArrayTuple saltedHash)
        {
            var bytes = credentials.GetBytes();
            return SecureHash(bytes, saltedHash.Item1.ToArray()).SequenceEqual(saltedHash.Item2.ToArray());
        }

        public byte[] Encrypt(byte[] key, byte[] decrypted)
        {
            using var aes = CreateAlgorithm(key);
            using var encryptor = aes.CreateEncryptor();

            var iv = aes.IV;
            var cipher = Apply(encryptor, decrypted);

            return iv.Concat(cipher).ToArray();
        }

        public byte[] Decrypt(byte[] key, byte[] encrypted)
        {
            var iv = encrypted.Take(IV_LENGTH).ToArray();
            var cipher = encrypted.Skip(IV_LENGTH).ToArray();

            using var aes = CreateAlgorithm(key, iv);
            using var decryptor = aes.CreateDecryptor();

            return Apply(decryptor, cipher);
        }

        //

        private const int ITERATIONS = 100000;

        private const int SALT_LENGTH = 24; // bytes
        private const int SECURE_HASH_LENGTH = 24; // bytes
        private const int IV_LENGTH = 16; // bytes

        private const int AES_KEYSIZE = 256; // bits

        private static readonly SHA512 LARGE_HASH = SHA512.Create();
        private static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        private static byte[] GenerateSalt() =>
            GetRandomBytes(SALT_LENGTH);

        private static byte[] SecureHash(byte[] value, byte[] salt) =>
            DeriveBytes(value, salt, SECURE_HASH_LENGTH);

        private static byte[] GetRandomBytes(int count)
        {
            var bytes = new byte[count];
            RNG.GetBytes(bytes);
            return bytes;
        }

        private static byte[] LargeHash(byte[] value) =>
            LARGE_HASH.ComputeHash(value);

        private static byte[] DeriveBytes(byte[] key, byte[] salt, int byteCount) =>
            new Rfc2898DeriveBytes(key, salt, ITERATIONS).GetBytes(byteCount);

        private static Aes CreateAlgorithm(byte[] key, byte[] iv = null)
        {
            var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");

            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = AES_KEYSIZE;
            aes.Key = key;
            aes.IV = iv ?? GetRandomBytes(IV_LENGTH);

            return aes;
        }

        private static byte[] Apply(ICryptoTransform transform, byte[] value)
        {
            using var output = new MemoryStream();

            using (var cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write))
                cryptoStream.Write(value, 0, value.Length);

            return output.ToArray();
        }
    }
}