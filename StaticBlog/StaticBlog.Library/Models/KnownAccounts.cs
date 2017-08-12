using System.Collections.Generic;

namespace StaticBlog.Library.Models
{
    public static class KnownAccounts
    {
        public static List<KeyValuePair<string, string>> List => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("marcel", "123456"),
        };
    }
}