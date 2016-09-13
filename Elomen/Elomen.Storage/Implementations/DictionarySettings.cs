using System;
using System.Collections.Generic;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class DictionarySettings : CompositeSettings
    {
        public string this[string key]
        {
            get { return dict.ContainsKey(key) ? dict[key] : ""; }
            set { dict[key] = value; }
        }

        public IEnumerable<string> GetKeys()
        {
            return dict.Keys;
        }

        //

        private readonly Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}