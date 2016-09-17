using System;
using System.Diagnostics;
using CoreTweet;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class ConsoleAuthorizer : Authorizable
    {
        public Tokens Authorize(string key, string secret)
        {
            var session = OAuth.Authorize(key, secret);
            Process.Start(session.AuthorizeUri.AbsoluteUri);

            Console.Write("Enter PIN: ");
            var pin = Console.ReadLine();

            return session.GetTokens(pin);
        }
    }
}