using System;
using System.Collections.Generic;
using Secure.Contracts;

namespace Secure.Services
{
    public class TransformerFactory
    {
        public ITransformer<string, string, string> Create(string arg)
        {
            var key = arg.ToLowerInvariant().Substring(0, 3);
            return MAP.ContainsKey(key) ? MAP[key] : throw new InvalidOperationException($"Unknown command: {arg}");
        }

        //

        private static readonly Dictionary<string, ITransformer<string, string, string>> MAP = new Dictionary<string, ITransformer<string, string, string>>
        {
            { "enc", new EncryptAndEncode() },
            { "dec", new DecodeAndDecrypt() },
        };
    }
}