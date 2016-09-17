using System;
using System.Reactive.Linq;
using CoreTweet.Streaming;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;
using Elomen.TwitterLibrary.Implementations;

namespace Elomen.Spike
{
    internal class Program
    {
        private const string APP_PREFIX = "Elomen.";
        private const string FILENAME = "settings.dat";

        private static void Main()
        {
            var appSettings = GetAppStore(APP_PREFIX).Load();
            var userStore = GetUserStore(FILENAME);

            var loader = new DefaultTokenLoader(appSettings, userStore.Load());
            var authorizer = new ConsoleAuthorizer(appSettings);

            var safeLoader = new AuthorizingTokenLoader(loader, authorizer, userStore);
            var tokens = safeLoader.Load();

            var stream = tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>();

            using (stream.Subscribe(msg => Console.WriteLine(msg.Status.User.ScreenName + ": " + msg.Status.Text)))
            {
                Console.ReadLine();
            }
        }

        //

        private static ResourceStore<CompositeSettings> GetAppStore(string prefix)
        {
            return new EnvironmentStore(EnvironmentVariableTarget.User, prefix, DictionarySettingsFactory.INSTANCE);
        }

        private static ResourceStore<CompositeSettings> GetUserStore(string filename)
        {
            var store = new FileStore(filename);

            var encryptor = new UserEncryptor();
            var encryptedStore = new EncodedStore<string, string>(store, encryptor);

            var encoder = new XmlSettingsEncoder(DictionarySettingsFactory.INSTANCE);
            return new EncodedStore<CompositeSettings, string>(encryptedStore, encoder);
        }
    }
}