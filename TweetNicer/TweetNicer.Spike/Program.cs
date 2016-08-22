using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using TweetNicer.Library.Implementations;

namespace TweetNicer.Spike
{
    internal class Program
    {
        private const string PASSWORD = "{8E63B644-29FD-47E5-B1A6-1DBFE7F3DF42}";

        private static void Main(string[] args)
        {
            var envStorage = new EnvironmentStorage(() => new DictionarySettings());
            var consumerVars = envStorage.LoadUserValues("TweetNicer.");

            var tokens = GetTokens(consumerVars["ConsumerKey"], consumerVars["ConsumerSecret"]);

            //var stream = tokens.Streaming.UserAsObservable().Publish();

            ////stream.OfType<FriendsMessage>()
            ////    .Subscribe(x => Console.WriteLine("Following: " + string.Join(", ", x)));

            //stream.OfType<StatusMessage>()
            //    .Subscribe(x => Console.WriteLine(Align(x.Status.User.ScreenName + ": ", x.Status.Text)));

            //using (stream.Connect())
            //    Console.ReadLine();

            //var stream = tokens
            //    .Streaming
            //    .FilterAsObservable(track => "football")
            //    .OfType<StatusMessage>();

            var stream = tokens
                .Streaming
                .UserAsObservable()
                .OfType<StatusMessage>();

            using (stream.Subscribe(msg => Console.WriteLine(Align(msg.Status.User.ScreenName + ": ", msg.Status.Text, Console.WindowWidth))))
            {
                Console.ReadLine();
            }
        }

        private static Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            var storage = new WindowsSecureStorage(new WindowsFileSystem(), new WindowsDataProtector(), PASSWORD);
            var userSettings = new EncodedSettings(storage, new SettingsEncoder(() => new DictionarySettings()));

            try
            {
                var settings = userSettings.LoadUserValues("user.settings");

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
                userSettings.SaveUserValues("user.settings", settings);

                return tokens;
            }
        }

        private static string Align(string prefix, string text, int length = 80)
        {
            Debug.Assert(prefix != null);
            Debug.Assert(text != null);
            Debug.Assert(length > prefix.Length);

            var all = prefix + text;

            return all.Length <= length
                ? all
                : all.Substring(0, length) +
                  Align(new string(' ', prefix.Length), all.Substring(length), length);
        }
    }
}