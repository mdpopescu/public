using System;
using System.Configuration;
using System.Diagnostics;
using System.Reactive.Linq;
using CoreTweet;
using CoreTweet.Streaming;
using TweetNicer.Library.Implementations;

namespace TweetNicer.Spike
{
    class Program
    {
        private static void Main(string[] args)
        {
            var settings = ConfigurationManager.AppSettings;

            var tokens = GetTokens(settings["ConsumerKey"], settings["ConsumerSecret"]);

            var stream = tokens.Streaming.UserAsObservable().Publish();

            //stream.OfType<FriendsMessage>()
            //    .Subscribe(x => Console.WriteLine("Following: " + string.Join(", ", x)));

            stream.OfType<StatusMessage>()
                .Subscribe(x => Console.WriteLine(Align(x.Status.User.ScreenName + ": ", x.Status.Text)));

            using (stream.Connect())
                Console.ReadLine();

            //var stream = tokens
            //    .Streaming
            //    .FilterAsObservable(track => "football")
            //    .OfType<StatusMessage>();

            //using (stream.Subscribe(msg => Console.WriteLine(Align(msg.Status.User.ScreenName + ": ", msg.Status.Text))))
            //{
            //    Console.ReadLine();
            //}
        }

        private static Tokens GetTokens(string consumerKey, string consumerSecret)
        {
            var storage = new WindowsSecureStorage(new WindowsFileSystem(), new WindowsDataProtector());
            var userSettings = new FileSettings(storage, new SettingsSerializer(() => new InMemorySettings()));

            try
            {
                var settings = userSettings.LoadUserSettings("user.settings");

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

                var settings = new InMemorySettings
                {
                    ["AccessToken"] = tokens.AccessToken,
                    ["AccessTokenSecret"] = tokens.AccessTokenSecret,
                };
                userSettings.SaveUserSettings("user.settings", settings);

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