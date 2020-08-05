using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SecurePasswordStorage.Library.Models;

namespace SecurePasswordStorage.Tests
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class ObjectMother
    {
        public static readonly Random RND = new Random();

        public static readonly char[] LETTERS_AND_DIGITS;
        public static readonly char[] PRINTABLE;

        static ObjectMother()
        {
            LETTERS_AND_DIGITS = new[]
            {
                Enumerable.Range('A', 26),
                Enumerable.Range('a', 26),
                Enumerable.Range('0', 10),
            }.ToCharArray();

            PRINTABLE = new[]
            {
                Enumerable.Range(' ', 95),
            }.ToCharArray();
        }

        public static Credentials CreateCredentials() =>
            new Credentials
            {
                Username = CreateString(10),
                Password = CreatePassword(10),
            };

        public static string CreateString(int length) =>
            CreateStringFromAlphabet(length, LETTERS_AND_DIGITS);

        public static string CreatePassword(int length) =>
            CreateStringFromAlphabet(length, PRINTABLE);

        public static EncryptedCredentials CreateEncryptedCredentials() =>
            new EncryptedCredentials
            {
                Encrypted = CreateBytes(16),
                Hashed = CreateSecureHash(),
            };

        public static SecureHash CreateSecureHash() =>
            new SecureHash();

        public static LargeHash CreateLargeHash() =>
            new LargeHash
            {
                PartOne = CreateKey(),
                PartTwo = CreateKey(),
            };

        public static Key CreateKey() =>
            new Key { Bytes = CreateBytes(16) };

        //

        private static char[] ToCharArray(this IEnumerable<IEnumerable<int>> enumerables) =>
            enumerables
                .SelectMany(i => i)
                .Select(i => (char) i)
                .ToArray();

        private static string CreateStringFromAlphabet(int length, IReadOnlyList<char> alphabet)
        {
            var chars = Enumerable
                .Range(1, length)
                .Select(_ => alphabet[RND.Next(alphabet.Count)])
                .ToArray();
            return new string(chars);
        }

        private static byte[] CreateBytes(int count) =>
            Enumerable.Range(1, count).Select(_ => (byte) RND.Next(256)).ToArray();
    }
}