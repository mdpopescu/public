using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SecurePasswordStorage.Library.Contracts;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Library.Services
{
    public class CryptoImpl : ICryptoFacade
    {
        public byte[] GenerateSalt()
        {
            var salt = new byte[SALT_LENGTH];
            RNG.GetBytes(salt);
            return salt;
        }

        public void Transform(Credentials credentials, out byte[] secretKey, out byte[] verificationHash)
        {
            var bytes = Encoding.UTF8.GetBytes(credentials.Key.Value)
                .Concat(Encoding.UTF8.GetBytes(credentials.Password))
                .ToArray();
            var largeHash = LargeHash(bytes);
            var hash1 = largeHash.Take(largeHash.Length / 2).ToArray();
            var hash2 = largeHash.Skip(largeHash.Length / 2).ToArray();

            secretKey = SecureHash(hash1);
            verificationHash = SecureHash(hash2);
        }

        public bool VerifyHash(byte[] expected, byte[] actual)
        {
            var expectedSalt = expected.Take(SALT_LENGTH).ToArray();
            var expectedHash = expected.Skip(SALT_LENGTH).ToArray();

            var actualSalt = actual.Take(SALT_LENGTH).ToArray();
            var actualValue = actual.Skip(SALT_LENGTH).ToArray();

            return expectedSalt.SequenceEqual(actualSalt)
                && expectedHash.SequenceEqual(DeriveBytes(actualValue, actualSalt, SECURE_HASH_LENGTH));
        }

        public byte[] Encrypt(byte[] key, byte[] decrypted)
        {
            using var aes = CreateAlgorithm(key);
            using var encryptor = aes.CreateEncryptor();

            var cipher = InternalEncrypt(encryptor, decrypted);
            var iv = aes.IV;

            var result = new byte[iv.Length + cipher.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, result, iv.Length, cipher.Length);

            return result;
        }

        public byte[] Decrypt(byte[] key, byte[] encrypted)
        {
            var iv = new byte[16];
            var cipher = new byte[encrypted.Length - iv.Length];

            Buffer.BlockCopy(encrypted, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encrypted, iv.Length, cipher, 0, cipher.Length);

            using var aes = CreateAlgorithm(key, iv);
            using var decryptor = aes.CreateDecryptor();

            return InternalDecrypt(decryptor, cipher);
        }

        //

        private const int ITERATIONS = 10000;
        private const int SALT_LENGTH = 24;
        private const int SECURE_HASH_LENGTH = 24;

        private static readonly SHA512 LARGE_HASH = SHA512.Create();
        private static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        private static byte[] LargeHash(byte[] value) =>
            LARGE_HASH.ComputeHash(value);

        private byte[] SecureHash(byte[] value)
        {
            var salt = GenerateSalt();
            var hash = DeriveBytes(value, salt, SECURE_HASH_LENGTH);
            return salt.Concat(hash).ToArray();
        }

        private Aes CreateAlgorithm(byte[] key)
        {
            var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");

            aes.Key = GetKey(key, aes.KeySize / 8);
            aes.GenerateIV();

            return aes;
        }

        private Aes CreateAlgorithm(byte[] key, byte[] iv)
        {
            var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");

            aes.Key = GetKey(key, aes.KeySize / 8);
            aes.IV = iv;

            return aes;
        }

        private byte[] GetKey(byte[] key, int byteCount)
        {
            var salt = GenerateSalt();
            return DeriveBytes(key, salt, byteCount);
        }

        private static byte[] DeriveBytes(byte[] key, byte[] salt, int byteCount)
        {
            var rfcKey = new Rfc2898DeriveBytes(key, salt, ITERATIONS);
            return rfcKey.GetBytes(byteCount);
        }

        private static byte[] InternalEncrypt(ICryptoTransform encryptor, byte[] value)
        {
            using var ms = new MemoryStream();
            using var stream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            stream.Write(value);
            return ms.ToArray();
        }

        private static byte[] InternalDecrypt(ICryptoTransform decryptor, byte[] value)
        {
            using var ms = new MemoryStream(value);
            using var stream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}