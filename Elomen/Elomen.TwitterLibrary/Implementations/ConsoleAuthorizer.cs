using System;
using System.Diagnostics;
using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class ConsoleAuthorizer : Authorizable
    {
        public Tokens Authorize(CompositeSettings appSettings)
        {
            var session = OAuth.Authorize(appSettings["ConsumerKey"], appSettings["ConsumerSecret"]);
            Process.Start(session.AuthorizeUri.AbsoluteUri);

            Console.Write("Enter PIN: ");
            var pin = Console.ReadLine();

            return session.GetTokens(pin);
        }
    }
}