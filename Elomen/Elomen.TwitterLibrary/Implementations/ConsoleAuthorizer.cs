using System;
using System.Diagnostics;
using CoreTweet;
using Elomen.Storage.Contracts;
using Elomen.TwitterLibrary.Contracts;

namespace Elomen.TwitterLibrary.Implementations
{
    public class ConsoleAuthorizer : Authorizable
    {
        public ConsoleAuthorizer(CompositeSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public Tokens Authorize()
        {
            var session = OAuth.Authorize(appSettings["ConsumerKey"], appSettings["ConsumerSecret"]);
            Process.Start(session.AuthorizeUri.AbsoluteUri);

            Console.Write("Enter PIN: ");
            var pin = Console.ReadLine();

            return session.GetTokens(pin);
        }

        //

        private readonly CompositeSettings appSettings;
    }
}