using System;
using CoreTweet;
using Elomen.Spike.Contracts;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;

namespace Elomen.Spike.Implementations
{
    public class TokenLoader
    {
        public TokenLoader(string filename, Approver approver)
        {
            this.filename = filename;
            this.approver = approver;
        }

        public Tokens Load()
        {
            var store = new EnvironmentStore(EnvironmentVariableTarget.User, "Elomen.", new DictionarySettingsFactory());

            var consumerVars = store.Load();
            return GetTokens(consumerVars["ConsumerKey"], consumerVars["ConsumerSecret"]);
        }

        //

        private readonly string filename;
        private readonly Approver approver;

        private Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            var store = new FileStore(filename);

            var encryptor = new UserEncryptor();
            var encryptedStore = new EncodedStore<string, string>(store, encryptor);

            var encoder = new XmlSettingsEncoder(new DictionarySettingsFactory());
            var userSettings = new EncodedStore<CompositeSettings, string>(encryptedStore, encoder);

            try
            {
                var settings = userSettings.Load();

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
                var pin = approver.Authorize(session.AuthorizeUri.AbsoluteUri);
                var tokens = session.GetTokens(pin);

                var settings = new DictionarySettings
                {
                    ["AccessToken"] = tokens.AccessToken,
                    ["AccessTokenSecret"] = tokens.AccessTokenSecret,
                };
                userSettings.Save(settings);

                return tokens;
            }
        }
    }
}