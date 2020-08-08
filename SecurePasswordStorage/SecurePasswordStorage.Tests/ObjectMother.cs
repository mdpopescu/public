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
            new Credentials(new UserKey(CreateString()), CreatePassword());

        public static User CreateUser(UserKey key) =>
            new User(key, CreateBytes(), CreateBytes());

        public static string CreateString() =>
            CreateStringFromAlphabet(LETTERS_AND_DIGITS);

        public static string CreatePassword() =>
            CreateStringFromAlphabet(PRINTABLE);

        public static string CreateStringFromAlphabet(IReadOnlyList<char> alphabet)
        {
            var length = RND.Next(10, 16);
            var chars = Enumerable
                .Range(1, length)
                .Select(_ => alphabet[RND.Next(alphabet.Count)])
                .ToArray();
            return new string(chars);
        }

        public static byte[] CreateBytes() =>
            Enumerable.Range(1, RND.Next(16, 32)).Select(_ => (byte) RND.Next(256)).ToArray();

        //

        private static char[] ToCharArray(this IEnumerable<IEnumerable<int>> enumerables) =>
            enumerables
                .SelectMany(i => i)
                .Select(i => (char) i)
                .ToArray();
    }
}