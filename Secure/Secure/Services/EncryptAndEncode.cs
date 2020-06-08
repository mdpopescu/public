using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Secure.Contracts;

namespace Secure.Services
{
    public class EncryptAndEncode : ITransformer<string, string, string>
    {
        public string Transform(string context, string value) => Encode(Encrypt(context, value));

        //

        private static byte[] Encrypt(string password, string text)
        {
            using var aes = CreateAlgorithm(password);
            using var encryptor = aes.CreateEncryptor();

            var cipher = InternalEncrypt(encryptor, text);
            var iv = aes.IV;

            var result = new byte[iv.Length + cipher.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, result, iv.Length, cipher.Length);

            return result;
        }

        private static Aes CreateAlgorithm(string password)
        {
            var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");

            aes.Key = GetKey(password, aes.KeySize / 8);
            aes.GenerateIV();

            return aes;
        }

        private static byte[] GetKey(string password, int byteCount)
        {
            var salt = Encoding.UTF8.GetBytes(Constants.SALT);
            var rfcKey = new Rfc2898DeriveBytes(password, salt, Constants.ITERATIONS);
            return rfcKey.GetBytes(byteCount);
        }

        private static byte[] InternalEncrypt(ICryptoTransform encryptor, string text)
        {
            using var ms = new MemoryStream();

            using (var stream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(stream))
                writer.Write(text);

            return ms.ToArray();
        }

        private static string Encode(byte[] buffer) => Convert.ToBase64String(buffer);
    }
}