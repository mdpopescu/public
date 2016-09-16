using System;
using System.Diagnostics;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using Elomen.Storage.Implementations;

namespace Elomen.Spike
{
    internal class Program
    {
        private const string FILENAME = "settings.dat";
        private const string PASSWORD = "{9CF76A0A-A773-4631-9CB0-BA5F4FBC1AF0}";

        private static void Main()
        {
            var consumerVars = new CompositeStorage("Elomen.", new EnvironmentStorage(), () => new DictionarySettings());
            var tokens = GetTokens(consumerVars.UserValues["ConsumerKey"], consumerVars.UserValues["ConsumerSecret"]);

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
            var store = new FileStore(new WindowsFileSystem(), FILENAME);
            var secureStorage = new WindowsSecureStorage(store, new WindowsDataEncryptor(PASSWORD));
            var userSettings = new EncodedSettings(secureStorage, new SettingsEncoder(() => new DictionarySettings()));

            try
            {
                var settings = userSettings.UserValues;

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
                userSettings.UserValues = settings;

                return tokens;
            }
        }
    }
}