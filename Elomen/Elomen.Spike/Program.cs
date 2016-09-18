using System;
using Elomen.Library.Implementations;
using Elomen.Spike.Implementations;
using Elomen.Storage.Contracts;
using Elomen.Storage.Implementations;
using Elomen.TwitterLibrary.Implementations;

namespace Elomen.Spike
{
    internal class Program
    {
        private const string APP_PREFIX = "Elomen.";
        private const string FILENAME = "ElomenBot.dat";

        private static void Main()
        {
            var appSettings = GetAppStore(APP_PREFIX).Load();
            var userStore = GetUserStore(FILENAME);

            var loader = new DefaultTokenLoader(appSettings, userStore);
            var authorizer = new ConsoleAuthorizer(appSettings);

            var safeLoader = new AuthorizingTokenLoader(loader, authorizer, userStore);
            var tokens = safeLoader.Load();
            var channel = new TwitterChannel(tokens);

            var interpreter = new Interpreter(new FakeCommandParser());
            var monitor = new ChannelMonitor(interpreter);

            Console.WriteLine("Monitoring...");
            monitor.Monitor(channel);
            Console.ReadLine();
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