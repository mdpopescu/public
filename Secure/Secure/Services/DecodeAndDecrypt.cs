using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Secure.Contracts;

namespace Secure.Services
{
    public class DecodeAndDecrypt : ITransformer<string, string, string>
    {
        public string Transform(string context, string value) => Decrypt(context, Decode(value));

        //

        private static byte[] Decode(string value) => Convert.FromBase64String(value);

        private static string Decrypt(string password, byte[] buffer)
        {
            var iv = new byte[16];
            var cipher = new byte[buffer.Length - iv.Length];

            Buffer.BlockCopy(buffer, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(buffer, iv.Length, cipher, 0, cipher.Length);

            using var aes = CreateAlgorithm(password, iv);
            using var decryptor = aes.CreateDecryptor();

            return InternalDecrypt(decryptor, cipher);
        }

        private static Aes CreateAlgorithm(string password, byte[] iv)
        {
            var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");

            aes.Key = GetKey(password, aes.KeySize / 8);
            aes.IV = iv;

            return aes;
        }

        private static byte[] GetKey(string password, int byteCount)
        {
            var salt = Encoding.UTF8.GetBytes(Constants.SALT);
            var rfcKey = new Rfc2898DeriveBytes(password, salt, Constants.ITERATIONS);
            return rfcKey.GetBytes(byteCount);
        }

        private static string InternalDecrypt(ICryptoTransform decryptor, byte[] cipher)
        {
            using var ms = new MemoryStream(cipher);
            using var stream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}