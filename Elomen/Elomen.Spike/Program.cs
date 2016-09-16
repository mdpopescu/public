using System;
using System.Diagnostics;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;

namespace Elomen.Spike
{
    internal class Program
    {
        private const string FILENAME = "settings.dat";

        private static void Main()
        {
            var store = new EnvironmentStore(EnvironmentVariableTarget.User, "Elomen.", new DictionarySettingsFactory());

            var consumerVars = store.Load();
            var tokens = GetTokens(consumerVars["ConsumerKey"], consumerVars["ConsumerSecret"]);

            var stream = tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>();

            using (stream.Subscribe(msg => Console.WriteLine(msg.Status.User.ScreenName + ": " + msg.Status.Text)))
            {
                Console.ReadLine();
            }
        }

        private static Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            var store = new FileStore(FILENAME);

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
                Process.Start(session.AuthorizeUri.AbsoluteUri);
                Console.Write("Enter PIN: ");
                var pin = Console.ReadLine();
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