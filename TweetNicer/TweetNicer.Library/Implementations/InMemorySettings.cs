using System;
using System.Collections.Generic;
using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
{
    public class InMemorySettings : Settings
    {
        public string this[string key]
        {
            get { return dict.ContainsKey(key) ? dict[key] : null; }
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