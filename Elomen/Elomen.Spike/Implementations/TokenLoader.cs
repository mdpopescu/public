using System;
using CoreTweet;
using Elomen.Spike.Contracts;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;

namespace Elomen.Spike.Implementations
{
    public class TokenLoader
    {
        public TokenLoader(ResourceStore<CompositeSettings> appStore, ResourceStore<CompositeSettings> userStore, Authorizable authorizable)
        {
            this.appStore = appStore;
            this.userStore = userStore;
            this.authorizable = authorizable;
        }

        public Tokens Load()
        {
            var consumerVars = appStore.Load();
            return GetTokens(consumerVars["ConsumerKey"], consumerVars["ConsumerSecret"]);
        }

        //

        private readonly ResourceStore<CompositeSettings> appStore;
        private readonly ResourceStore<CompositeSettings> userStore;
        private readonly Authorizable authorizable;

        private Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            try
            {
                var settings = userStore.Load();

                return new Tokens
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    AccessToken = settings["AccessToken"],
                    AccessTokenSecret = settings["AccessTokenSecret"],
                };
            }
            catch (Exception)
            {
                var session = OAuth.Authorize(consumerKey, consumerSecret);
                var pin = authorizable.Authorize(session.AuthorizeUri.AbsoluteUri);
                var tokens = session.GetTokens(pin);

                SaveUserSettings(tokens);

                return tokens;
            }
        }

        private void SaveUserSettings(Tokens tokens)
        {
            var settings = new DictionarySettings
            {
                ["AccessToken"] = tokens.AccessToken,
                ["AccessTokenSecret"] = tokens.AccessTokenSecret,
            };
            userStore.Save(settings);
        }
    }
}