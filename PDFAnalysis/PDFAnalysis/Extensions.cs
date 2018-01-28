using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace PDFAnalysis
{
    public static class Extensions
    {
        public static string Get(this IDictionary<string, string> dict, string key) =>
            dict.ContainsKey(key) ? dict[key] : "";

        public static IEnumerable<string> Quoted(this IEnumerable<string> values) =>
            values.Select(value => Q + value.Replace(Q, Q + Q) + Q);

        public static IEnumerable<string> Extract(this IDictionary<string, string> dict, IEnumerable<string> keys) =>
            keys.Select(dict.Get);

        public static IEnumerable<int> FindAll(this string s, string fragment)
        {
            var start = 0;
            while (start <= s.Length)
            {
                var index = s.IndexOf(fragment, start, StringComparison.Ordinal);
                if (index == -1)
                    break;

                yield return index;
                start = index + fragment.Length;
            }
        }

        public static string GetMD5(this byte[] bytes)
        {
            using (var md5 = MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(bytes));
        }

        //

        private const string Q = "\"";
    }
}