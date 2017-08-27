using System.Collections.Generic;

namespace StaticBlog.Library.Models
{
    public class Account
    {
        public static List<Account> KnownAccounts => new List<Account>
        {
            new Account("marcel", "123456"),
        };

        public Account(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public bool Matches(Account other)
        {
            return username == other.username && password == other.password;
        }

        //

        private readonly string username;
        private readonly string password;
    }
}